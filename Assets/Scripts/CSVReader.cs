using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : MonoBehaviour {

	// fileNameのn行目の値を配列として返す
	public string[] CSVReadLine(string fileName, int n){
		StreamReader sr = new StreamReader("Assets/Resources/CSV/"+fileName+".csv");
		string line;
		int i = 0;
		string[] fields = new string[0];
		while ((line = sr.ReadLine()) != null){
			if (i == n){
				fields = line.Split(',');
				break;
			}
			i++;
		}
		return fields;
	}
}