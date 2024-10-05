using System;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class FriendManager : MonoBehaviour
{
    [SerializeField]
    private int initialFriendBurstCount = 0;
    [SerializeField]
    private float initialFriendBurstRate = 0;

    [SerializeField]
    private ComputeShader updateShader = null;
    [SerializeField]
    private ComputeShader renderShader = null;

    [SerializeField]
    private RawImage output = null;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct Friend {
        public Vector2 position;
        public Vector2 velocity;
        public Vector3 color;
        public float decisionTimer;
    };

    private bool initialized = false;
    private RenderTexture renderTexture;
    private int friendCount = 0;
    private ComputeBuffer friendDataBuffer;
    private int updateFriendsKernel;
    private int renderFriendsKernel;
    private uint updateThreadGroupSize;
    private uint renderThreadGroupSize;
    private int updateDispatchSize;
    private int renderDispatchSize;

    private const int FRIEND_STRIDE = sizeof(float) * 8;
    private const int FRIEND_MAX = 100000000 / FRIEND_STRIDE;

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
                color = new Vector3(UnityEngine.Random.Range(0.6f, 1.0f), UnityEngine.Random.Range(0.6f, 1.0f), UnityEngine.Random.Range(0.6f, 1.0f)),
                decisionTimer = 0.0f
            };
        }

        friendDataBuffer = new ComputeBuffer(FRIEND_MAX, FRIEND_STRIDE);
        friendDataBuffer.SetData(friends);

        updateFriendsKernel = updateShader.FindKernel("UpdateFriends");
        renderFriendsKernel = renderShader.FindKernel("RenderFriends");

        updateShader.SetInt("screenWidth", Screen.width);
        updateShader.SetInt("screenHeight", Screen.height);
        updateShader.SetInt("frameCount", 0);
        updateShader.SetFloat("deltaTime", 0.0f);
        updateShader.SetBuffer(updateFriendsKernel, "friendData", friendDataBuffer);
        updateShader.SetInt("friendCount", friendCount);
        renderShader.SetTexture(renderFriendsKernel, "renderTexture", renderTexture);
        renderShader.SetInt("screenWidth", Screen.width);
        renderShader.SetInt("screenHeight", Screen.height);
        renderShader.SetInt("frameCount", 0);
        renderShader.SetBuffer(renderFriendsKernel, "friendData", friendDataBuffer);
        renderShader.SetInt("friendCount", friendCount);

        updateShader.GetKernelThreadGroupSizes(updateFriendsKernel, out updateThreadGroupSize, out _, out _);
        renderShader.GetKernelThreadGroupSizes(renderFriendsKernel, out renderThreadGroupSize, out _, out _);
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

        updateShader.SetInt("frameCount", Time.frameCount);
        updateShader.SetFloat("deltaTime", Time.deltaTime);
        updateShader.SetInt("friendCount", friendCount);
        updateDispatchSize = Mathf.CeilToInt((float) friendCount / updateThreadGroupSize);
        updateShader.Dispatch(updateFriendsKernel, updateDispatchSize, 1, 1);
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = oldRT;
        renderShader.SetInt("frameCount", Time.frameCount);
        renderShader.SetInt("friendCount", friendCount);
        renderDispatchSize = Mathf.CeilToInt((float) friendCount / renderThreadGroupSize);
        renderShader.Dispatch(renderFriendsKernel, renderDispatchSize, 1, 1);
    }
}
