using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancePointer<T> where T : MonoBehaviour
{

	T _instance;
    public T Get()
	{
		return _instance ?? (_instance = GameObject.FindObjectOfType<T>());
	}
}
