using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class dataBaseManager : MonoBehaviour
{
    //Clases para datos del json

    [Serializable]
    public class Data {
        public string nombre = "";
        public string tipo = "";
        public string NN = "";
        public string PK = "";
        public string AI = "";
        public string size = "";
        public string fkTable = "";
    }

    [Serializable]
    public class Columnas {
        public string nombre = "";
        public Data[] colum;
    }

    [Serializable]
    public class Tablas {
        public Columnas[] tabla;
    }

    //Clases para datos de la bd

    [Serializable]
    public class DataDB {
        public string cid = "";
        public string name = "";
        public string type = "";
        public string notnull = "";
        public string dflt_value = "";
        public string pk = "";
    }
    [Serializable]
    public class ColumnasDB {
        public DataDB[] colums;
    }


    public string[] ordenTablas;
    string path;
    string jsonString;
    bool tableFound = false;
    bool columFound = false;

    private void Awake() {
        ordenTablas = new string[] { "usuario", "catalogoAcciones", "catalogoCatgoriaPaquete", "catalogoEjercicio", "codigo", "paquete", "descarga", "aviso", "estadistica", "log", "preferencias", "intento", "registros", "pregunta", "respuesta", "detalleIntento" };
        tableFound = false;
        columFound = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        path = Application.streamingAssetsPath + "/dataBase.json";
        jsonString = File.ReadAllText(path);
        Tablas tablasJson = JsonUtility.FromJson<Tablas>(jsonString);
        for (int i = 0; i < ordenTablas.Length; i++) {
            foreach (var tabJson in tablasJson.tabla) {
                if (tabJson.nombre == ordenTablas[i]) {
                    string res = consultarTablas(tabJson.nombre);
                    if (res == "0") {
                        Debug.Log("No encontro la tabla " + tabJson.nombre);
                        Debug.Log("Crear tabla en la bd");
                    } else {
                        tableFound = true;
                        ColumnasDB columnasDB = JsonUtility.FromJson<ColumnasDB>("{\"colums\" : [" + res + "]}");
                        foreach (var colDB in columnasDB.colums) {
                            foreach (var colJson in tabJson.colum) {
                                if (colDB.name == colJson.nombre) {
                                    columFound = true;
                                    Debug.Log(colDB.name);
                                } else {
                                    Debug.Log("El nombre de la columna actual no coincide.");
                                }
                            }
                            if (columFound == false) {
                                Debug.Log("Borrar columna de la bd");
                            }
                            columFound = false;

                        }
                    }
                } else {
                    Debug.Log("El nombre de la tabla actual no coincide.");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string consultarTablas(string nombreTabla) {
        string query = "PRAGMA table_info("+nombreTabla+");";
        var result = conexionDB.selectGeneral(query);
        return result;
    }
}
