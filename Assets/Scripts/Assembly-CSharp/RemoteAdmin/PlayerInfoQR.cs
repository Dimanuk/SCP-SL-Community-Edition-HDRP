using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace RemoteAdmin
{
    internal class PlayerInfoQR : MonoBehaviour
    {
        public RawImage QrDisplay;

        private static PlayerInfoQR _singleton;
        private static BarcodeWriter _barcodeWriter;
        private static Texture2D _emptyCode;
        private static bool _clear;

        private const int Size = 125;

        public void OnEnable()
        {
            _singleton = this;
            _clear = false;

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

        public static void Display(string userId)
        {
            _clear = false;

            if (_barcodeWriter == null)
                return;

            var matrix = _barcodeWriter.Encode(userId);
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    tex.SetPixel(x, y, matrix[x, y] ? Color.black : Color.white);
                }
            }

            tex.Apply();
            if (_singleton != null && _singleton.QrDisplay != null)
            {
                _singleton.QrDisplay.enabled = true;
                _singleton.QrDisplay.texture = tex;
            }
        }

        public static void Clear()
        {
            if (_clear)
                return;

            _clear = true;

            if (_emptyCode == null)
            {
                _emptyCode = new Texture2D(Size, Size);
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        _emptyCode.SetPixel(x, y, Color.white);
                    }
                }
                _emptyCode.Apply();
            }

            if (_singleton != null && _singleton.QrDisplay != null)
            {
                _singleton.QrDisplay.enabled = true;
                _singleton.QrDisplay.texture = _emptyCode;
            }
        }
    }
}