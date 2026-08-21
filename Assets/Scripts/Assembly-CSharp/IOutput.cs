using System;
using UnityEngine;

public interface IOutput
{
	void Print(string text);

	void Print(string text, ConsoleColor c);

	void Print(string text, ConsoleColor c, Color rgbColor);
}
