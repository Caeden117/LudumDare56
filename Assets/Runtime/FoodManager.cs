using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class FoodManager : MonoBehaviour {
    [SerializeField]
    private TransparancyManager transparencyManager;
    [SerializeField]
    private RectTransform foodContainer;
    [SerializeField]
    private GameObject foodPrefab;

    private readonly string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
    private string foodFolder;
    private string eatenFolder;
    private bool saving = false;

    public Dictionary<string, FoodItem> Food { get; private set; } = new Dictionary<string, FoodItem>();

    private async UniTask Start() {
        foodPrefab.GetComponent<WindowInteractable>().transparancyManager = transparencyManager;

        // Path.GetFullPath to normalize path separators on Windows
        foodFolder = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "food"));
        eatenFolder = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "eaten"));

        Debug.Log(foodFolder);

        if (!Directory.Exists(foodFolder)) {
            Directory.CreateDirectory(foodFolder);
        }

        if (!File.Exists(Path.Combine(foodFolder, "Put images here.txt"))) {
            File.WriteAllText(Path.Combine(foodFolder, "Put images here.txt"), "Feed your friends by placing image files (PNG & JPEG) in this folder.\r\nWatch as they chip away pixel by pixel!\r\n(A backup of all images is placed in ../eaten/)");
        }

        if (!Directory.Exists(eatenFolder)) {
            Directory.CreateDirectory(eatenFolder);
        }

        while (true) {
            string[] imageNames = Directory
                .GetFiles(foodFolder)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .Select(Path.GetFileName)
                .ToArray();

            foreach (string imageName in imageNames) {
                if (!Food.ContainsKey(imageName)) {
                    LoadImage(imageName).Forget();
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1.0));
        }
    }

    public FoodItem GetFood() {
        if (Food.Count <= 0) {
            return null;
        }

        FoodItem foodItem = Food.First().Value;

        return foodItem.Ready ? foodItem : null;
    }

    public async UniTask OnAte(FoodItem foodItem) {
        foodItem.OnAte();
        File.Delete(Path.Combine(foodFolder, foodItem.Name));
        await UniTask.NextFrame();
        Food.Remove(foodItem.Name);
    }

    public void OpenFoodFolder() {
        Application.OpenURL(foodFolder);
    }

    private async UniTask LoadImage(string imageName) {
        Food[imageName] = new FoodItem(imageName);

        // Make sure a backup of the file exists (no overwriting)
        if (!File.Exists(Path.Combine(eatenFolder, imageName))) {
            File.Copy(Path.Combine(foodFolder, imageName), Path.Combine(eatenFolder, imageName));
        }

        byte[] imageBytes = await File.ReadAllBytesAsync(Path.Combine(foodFolder, imageName));
        Texture2D tex2D = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex2D, imageBytes);
        Food[imageName].LoadTexture2D(tex2D);

        GameObject prefabInstance = Instantiate(foodPrefab, foodContainer);
        Food[imageName].SetPrefabInstance(prefabInstance);
    }

    public async UniTask SaveImage(FoodItem foodItem) {
        if (saving) return;
        saving = true;

        try {
            var format = foodItem.RT.graphicsFormat;
            var req = await UnityEngine.Rendering.AsyncGPUReadback.Request(foodItem.RT, 0);
            var buf = req.GetData<byte>().ToArray();
            var width = (uint)req.width;
            var height = (uint)req.height;
            await UniTask.SwitchToThreadPool();

            byte[] encoded;
            if (foodItem.IsPNG) {
                encoded = ImageConversion.EncodeArrayToPNG(buf, format, width, height);
            } else {
                encoded = ImageConversion.EncodeArrayToJPG(buf, format, width, height);
            }

            var path = Path.Combine(foodFolder, foodItem.Name);
            await File.WriteAllBytesAsync(path, encoded);
        } finally {
            saving = false;
            await UniTask.SwitchToMainThread();
        }
    }
}
