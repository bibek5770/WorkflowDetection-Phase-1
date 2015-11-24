using UnityEngine;
using System.Collections;

public class Properties : MonoBehaviour 
{
    public enum PropShape
	{
		Rectangular = 1,
		Spherical = 2,
		Cylindrical
	};
	public enum PropColor
	{
		Red = 1,
		Green = 2, 
		Blue = 3
	};
	public enum PropFunction
	{
		Function1 = 1,
		Function2 = 2
	};
	public enum PropTexture
	{
		Rough = 1,
		Smooth =2
	};
	public enum PropOpacity
	{
		Transparent = 1,
		Opaque = 2,
		Translucent = 3
	};

	public PropShape shape;
	public PropColor color;
	public PropFunction function;
	public PropTexture texture;
	public PropOpacity opacity;

}