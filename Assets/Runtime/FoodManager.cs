using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FoodManager : MonoBehaviour {
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject foodPrefab;

    private string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
    private string foodFolder;
    private string eatenFolder;

    public Dictionary<string, FoodItem> Food { get; private set; } = new Dictionary<string, FoodItem>();

    private async UniTask Start() {
        // Path.GetFullPath to normalize path separators on Windows
        foodFolder = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "food"));
        eatenFolder = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "eaten"));

        Debug.Log(foodFolder);

        if (!Directory.Exists(foodFolder)) {
            Directory.CreateDirectory(foodFolder);
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

    public void OpenFoodFolder() {
        System.Diagnostics.Process.Start("explorer.exe", foodFolder);
    }

    private async UniTask LoadImage(string imageName) {
        Food[imageName] = new FoodItem();

        // Make sure a backup of the file exists (no overwriting)
        if (!File.Exists(Path.Combine(eatenFolder, imageName))) {
            File.Copy(Path.Combine(foodFolder, imageName), Path.Combine(eatenFolder, imageName));
        }

        byte[] imageBytes = await File.ReadAllBytesAsync(Path.Combine(foodFolder, imageName));
        Texture2D tex2D = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex2D, imageBytes);
        Food[imageName].LoadTexture2D(tex2D);

        Food[imageName].SetPrefabInstance(Instantiate(foodPrefab, canvas.transform));
    }
}
