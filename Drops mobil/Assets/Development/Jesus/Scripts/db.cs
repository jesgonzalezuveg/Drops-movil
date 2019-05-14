using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using Random = System.Random;
using System.Dynamic;
using Object = System.Object;

public class db : MonoBehaviour
{
    String code ="";
    String code2 ="";

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Hola Mundo");
        //string query = "DELETE FROM codigo WHERE id=32;";
        //string query = "UPDATE codigo SET descripcion = 'test1111', status = 1 WHERE id = 32; ";
        //string query = "INSERT INTO codigo (descripcion, status, fechaRegistro, fechaModificacion) VALUES ('test', 0, datetime(), datetime());";
        //string query = "SELECT * FROM codigo";
        //Debug.Log(conexionDB.alterGeneral(query));
        //Debug.Log(conexionDB.selectGeneral(query));
        //code = generateCode();
        //StartCoroutine(WebServiceCodigo.obtenerCodigo("XdKpla", 1));
        //StartCoroutine(WebServiceCodigo.insertarCodigo(code));
        GameObject.FindGameObjectWithTag("codigo").GetComponent<Text>().text= code2;
    }

    // Update is called once per frame
    void Update()
    {

    }


    /** Funcion que sirve para generar un código de 8 caracteres de manera aleatoria
    *
    *@param  chars Lista de caracteres 
    *@param  stringChars Contenedor de los 6 caracteres que contendra el codigo
    *@param  random Funcion para elección aleatoria
    *@param  finalString Código obtentido
    **/
    public String generateCode()
    {
        var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var stringChars = new char[6];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
            if (i == 2)
            {
                code2 = code2 + stringChars[i] + "-";
            }
            else
            {
                code2 = code2 + stringChars[i];
            }
        }

        var finalString = new String(stringChars);
        string query = "INSERT INTO codigo (descripcion, status, fechaRegistro, fechaModificacion) VALUES ('"+ finalString + "', 0, datetime(), datetime())";
        var result = conexionDB.alterGeneral(query);

        if (result == 1)
        {
            Debug.Log("Se inserto correctamente");
            Debug.Log(finalString);
            Debug.Log(result);
            return finalString;
        }
        else
        {
            Debug.Log("Error al insertar");
            Debug.Log(result);
            return "Error al insertar";
        }
    }
}
