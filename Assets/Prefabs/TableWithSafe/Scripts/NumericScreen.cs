﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class NumericScreen : MonoBehaviour {

    public TextMeshPro numbersTextMesh;
    [SerializeField]
    private List<int> unlockCode = new List<int>();

    [Header("Code Entered Methods")]
    public UnityEvent correctCodeMethods;
    public UnityEvent incorrectCodeMethods;

    private List<int> numbers = new List<int>();

	void Awake () {
        ClearNumbers();
    }

    public void ClearNumbers() {
        numbersTextMesh.text = "";
        numbers.Clear();
        numbers = new List<int>();
    }

    public void InsertNumber(int number) {
        if (numbers.Count < unlockCode.Count) {
            numbers.Add(number);

            //Pinta el texto de los numeros
            string numbersText = "";
            foreach (int num in numbers)
                numbersText = numbersText + num + " ";
            numbersTextMesh.text = numbersText;

            //Comprueba si el codigo que ha sido introducido es correcto o no
            if (numbers.Count == unlockCode.Count) {
                bool correctCode = true;
                for (int i = 0; i< numbers.Count; i++){
                    if (numbers[i] != unlockCode[i]) {
                        incorrectCodeMethods.Invoke();
                        correctCode = false;
                        break;
                    }
                }

                if (correctCode)
                    correctCodeMethods.Invoke();
            }
        }
    }

}