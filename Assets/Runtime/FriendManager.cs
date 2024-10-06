using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FriendManager : MonoBehaviour
{
    public Friend[] RandomFriends = new Friend[RANDOM_FRIEND_COUNT];

    [SerializeField]
    private int initialFriendBurstCount = 0;
    [SerializeField]
    private float initialFriendBurstRate = 0;
    [SerializeField]
    private bool tiny = true;

    [SerializeField]
    private ComputeShader updateShader = null;
    [SerializeField]
    private ComputeShader renderShader = null;
    [SerializeField]
    private ComputeShader copyShader = null;

    [SerializeField]
    private RawImage output = null;

    [SerializeField]
    private WindowManager windowManager = null;

    [SerializeField]
    private List<string> customColorPalette = new List<string>();

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Friend {
        public Vector2 position;
        public Vector2 velocity;
        public uint packed0;
        public float fvar0;
        public float fvar1;
        public float fvar2;
        public float fvar3;

        public byte state {
            get {
                return (byte) (packed0 & 0xFu);
            }
            set {
                packed0 = (packed0 & ~0xFu) | (value & 0xFu);
            }
        }

        public byte mood {
            get {
                return (byte) ((packed0 >> 4) & 0xFFu);
            }
            set {
                packed0 = (packed0 & ~(0xFFu << 4)) | ((value & 0xFFu) << 4);
            }
        }

        public byte colorIdx {
            get {
                return (byte) ((packed0 >> 12) & 0xFFu);
            }
            set {
                packed0 = (packed0 & ~(0xFFu << 12)) | ((value & 0xFFu) << 12);
            }
        }
    };

    private bool initialized = false;
    private RenderTexture renderTexture;
    private int friendCount = 0;
    private ComputeBuffer friendDataBuffer;
    private ComputeBuffer colorPaletteBuffer;
    private ComputeBuffer copyFriendIdxBuffer;
    private ComputeBuffer copyFriendBuffer;
    private int updateFriendsKernel;
    private int renderFriendsKernel;
    private int copyFriendsKernel;
    private uint updateThreadGroupSize;
    private uint renderThreadGroupSize;
    private uint copyThreadGroupSize;
    private int updateDispatchSize;
    private int renderDispatchSize;

    private const int FRIEND_STRIDE = sizeof(float) * 8 + sizeof(int);
    private const int FRIEND_MAX = 100000000 / FRIEND_STRIDE;
    private const int RANDOM_FRIEND_COUNT = 64;
    private const int COLOR_PALETTE_STRIDE = sizeof(float) * 3;
    private const int COLOR_PALETTE_MAX = 1 << 8;

    private readonly int[] randomFriendIdx = new int[RANDOM_FRIEND_COUNT];

    private void OnEnable() {
        if (initialized) {
            OnDisable();
        }

        initialized = true;

        renderTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        output.texture = renderTexture;

        friendCount = 0;
        Friend[] friends = new Friend[FRIEND_MAX];
        for (int i = 0; i < initialFriendBurstCount; i++) {
            float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            float velocity = Random.Range(0.0f, 1000.0f);
            friends[i] = new Friend() {
                position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f),
                velocity = new Vector2(velocity * Mathf.Cos(angle), velocity * Mathf.Sin(angle)),
                packed0 = 0,
                fvar0 = 0.0f,
                fvar1 = 0.0f,
                fvar2 = 0.0f,
                fvar3 = 0.0f,

                state = 0,
                mood = (byte)Random.Range(127 - 40, 127 + 80), // starting mood: -40 to 80
                colorIdx = (byte) Random.Range(0, COLOR_PALETTE_MAX)
            };
        }

        friendDataBuffer = new ComputeBuffer(FRIEND_MAX, FRIEND_STRIDE);
        friendDataBuffer.SetData(friends);

        Vector3[] colorPalette = new Vector3[COLOR_PALETTE_MAX];
        for (int i = 0; i < COLOR_PALETTE_MAX; i++) {
            if (i < customColorPalette.Count) {
                Color color;
                if (ColorUtility.TryParseHtmlString(customColorPalette[i], out color)) {
                    colorPalette[i] = new Vector3(color.r, color.g, color.b);
                } else {
                    Debug.LogError("Failed to parse custom color palette color #" + i + ": " + customColorPalette[i]);
                }
            } else {
                colorPalette[i] = new Vector3(Random.Range(0.6f, 1.0f), Random.Range(0.6f, 1.0f), Random.Range(0.6f, 1.0f));
            }
        }

        colorPaletteBuffer = new ComputeBuffer(COLOR_PALETTE_MAX, COLOR_PALETTE_STRIDE);
        colorPaletteBuffer.SetData(colorPalette);

        copyFriendIdxBuffer = new ComputeBuffer(RANDOM_FRIEND_COUNT, sizeof(int));
        copyFriendBuffer = new ComputeBuffer(RANDOM_FRIEND_COUNT, FRIEND_STRIDE);

        updateFriendsKernel = updateShader.FindKernel("UpdateFriends");
        renderFriendsKernel = renderShader.FindKernel("RenderFriends");
        copyFriendsKernel = copyShader.FindKernel("CopyFriends");

        updateShader.SetInt("screenWidth", Screen.width);
        updateShader.SetInt("screenHeight", Screen.height);
        updateShader.SetInt("frameCount", 0);
        updateShader.SetFloat("deltaTime", 0.0f);
        updateShader.SetFloat("windowLeft", 0.0f);
        updateShader.SetFloat("windowTop", 0.0f);
        updateShader.SetFloat("windowRight", 0.0f);
        updateShader.SetFloat("windowBottom", 0.0f);
        updateShader.SetFloat("windowVelocityX", 0.0f);
        updateShader.SetFloat("windowVelocityY", 0.0f);
        updateShader.SetInt("friendCount", friendCount);
        updateShader.SetBuffer(updateFriendsKernel, "friendData", friendDataBuffer);

        renderShader.SetTexture(renderFriendsKernel, "renderTexture", renderTexture);
        renderShader.SetInt("screenWidth", Screen.width);
        renderShader.SetInt("screenHeight", Screen.height);
        renderShader.SetInt("frameCount", 0);
        renderShader.SetBool("tiny", tiny);
        renderShader.SetInt("friendCount", friendCount);
        renderShader.SetBuffer(renderFriendsKernel, "friendData", friendDataBuffer);
        renderShader.SetBuffer(renderFriendsKernel, "colorPalette", colorPaletteBuffer);

        copyShader.SetBuffer(copyFriendsKernel, "friendData", friendDataBuffer);
        copyShader.SetBuffer(copyFriendsKernel, "randomFriendData", copyFriendBuffer);
        copyShader.SetBuffer(copyFriendsKernel, "randomFriendIdx", copyFriendIdxBuffer);

        updateShader.GetKernelThreadGroupSizes(updateFriendsKernel, out updateThreadGroupSize, out _, out _);
        renderShader.GetKernelThreadGroupSizes(renderFriendsKernel, out renderThreadGroupSize, out _, out _);
        copyShader.GetKernelThreadGroupSizes(copyFriendsKernel, out copyThreadGroupSize, out _, out _);

        SelectNewRandomFriends();
    }

    public void SelectNewRandomFriends()
    {
        for (var i = 0; i < RANDOM_FRIEND_COUNT; i++)
        {
            randomFriendIdx[i] = Random.Range(0, friendCount);
        }

        copyFriendIdxBuffer.SetData(randomFriendIdx);

        CopyFriendsToCPU();
    }

    private void OnDisable() {
        if (initialized) {
            renderTexture.Release();
            friendDataBuffer.Release();
        }
        initialized = false;
    }

    private void LateUpdate() {
        if (friendCount < initialFriendBurstCount) {
            friendCount = Mathf.Min(initialFriendBurstCount, Mathf.FloorToInt(Time.time * initialFriendBurstRate));
        }

        if (friendCount <= 0)
        {
            return;
        }

        // Update loop
        updateShader.SetInt("frameCount", Time.frameCount);
        updateShader.SetFloat("deltaTime", Time.deltaTime);
        updateShader.SetFloat("windowLeft", windowManager.foregroundMin.x);
        updateShader.SetFloat("windowTop", windowManager.foregroundMin.y);
        updateShader.SetFloat("windowRight", windowManager.foregroundMax.x);
        updateShader.SetFloat("windowBottom", windowManager.foregroundMax.y);
        updateShader.SetFloat("windowVelocityX", windowManager.foregroundVelocity.x);
        updateShader.SetFloat("windowVelocityY", windowManager.foregroundVelocity.y);
        updateShader.SetInt("friendCount", friendCount);
        updateDispatchSize = Mathf.CeilToInt((float) friendCount / updateThreadGroupSize);
        updateShader.Dispatch(updateFriendsKernel, updateDispatchSize, 1, 1);

        // Copy loop
        CopyFriendsToCPU();

        // Render loop
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = oldRT;
        renderShader.SetInt("frameCount", Time.frameCount);
        renderShader.SetBool("tiny", tiny);
        renderShader.SetInt("friendCount", friendCount);
        renderDispatchSize = Mathf.CeilToInt((float) friendCount / renderThreadGroupSize);
        renderShader.Dispatch(renderFriendsKernel, renderDispatchSize, 1, 1);
    }

    private void CopyFriendsToCPU()
    {
        copyShader.Dispatch(copyFriendsKernel, RANDOM_FRIEND_COUNT, 1, 1);

        copyFriendBuffer.GetData(RandomFriends);
    }
}
