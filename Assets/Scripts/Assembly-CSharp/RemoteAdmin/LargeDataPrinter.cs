using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace RemoteAdmin
{
    public class LargeDataPrinter : MonoBehaviour
    {
        private const int Size = 700;

        internal static LargeDataPrinter Singleton;
        private static BarcodeWriter _barcodeWriter;

        public GameObject Panel;
        public TextMeshProUGUI Header;
        public TextMeshProUGUI Content;
        public RawImage QrDisplay;

        public void OnEnable()
        {
            Singleton = this;

            if (_barcodeWriter == null)
            {
                _barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Height = Size,
                        Width = Size
                    }
                };
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Panel != null && Panel.activeSelf)
                Panel.SetActive(false);
        }

        public static void Display(string content, bool replaceToBr)
        {
            if (_barcodeWriter == null || Singleton == null)
                return;

            if (replaceToBr)
                content = content.Replace("\n", "<br>");

            var matrix = _barcodeWriter.Encode(content);
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    tex.SetPixel(x, y, matrix[x, y] ? Color.black : Color.white);
                }
            }

            tex.Apply();

            Singleton.Panel.SetActive(true);
            Singleton.Content.SetText(content, true);
            Singleton.QrDisplay.texture = tex;
        }

        public static void Hide()
        {
            if (Singleton != null && Singleton.Panel != null)
                Singleton.Panel.SetActive(false);
        }
    }
}