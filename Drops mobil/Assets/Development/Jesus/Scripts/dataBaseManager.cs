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
                foreach (var col in tabJson.colum) {
                    crearColumna(col, tabJson.nombre);
                    return;
                }
                //if (tabJson.nombre == ordenTablas[i]) {
                //    string res = consultarTablas(tabJson.nombre);
                //    if (res == "0") {
                //        Debug.Log("No encontro la tabla " + tabJson.nombre);
                //        Debug.Log("Crear tabla en la bd");
                //    } else {
                //        tableFound = true;
                //        ColumnasDB columnasDB = JsonUtility.FromJson<ColumnasDB>("{\"colums\" : [" + res + "]}");

                //        //Elimar columanas que no esten en el json
                //        foreach (var colDB in columnasDB.colums) {
                //            foreach (var colJson in tabJson.colum) {
                //                if (colDB.name == colJson.nombre) {
                //                    columFound = true;
                //                } else {
                //                    Debug.Log("El nombre de la columna de la db actual no coincide.");
                //                }
                //            }
                //            if (columFound == false) {
                //                Debug.Log("Borrar columna de la bd");
                //            }
                //            columFound = false;
                //        }

                //        //Agregar y verificar estructura de tablas que contenga el json
                //        foreach (var colJson in tabJson.colum) {
                //            foreach (var colDB in columnasDB.colums) {
                //                if (colDB.name == colJson.nombre) {
                //                    columFound = true;
                //                } else {
                //                    Debug.Log("El nombre de la columna actual del json no coincide.");
                //                }
                //            }
                //            if (columFound == false) {
                //                Debug.Log("Agregar columna a la bd");
                //            }
                //            columFound = false;
                //        }
                //    }
                //} else {
                //    Debug.Log("El nombre de la tabla actual no coincide.");
                //}
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   void queryCrearTabla(Columnas tabla) {
        //string query = "CREATE TABLE IF NOT EXISTS \"aviso2\" (\"id\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"fechaVisualizacion\"    datetime NOT NULL, \"idPaquete\" int(11) NOT NULL, \"idUsuario\" int(11) NOT NULL, \"descripcion\"   VARCHAR(50), FOREIGN KEY(\"idUsuario\") REFERENCES \"usuario\"(\"id\"),FOREIGN KEY(\"idPaquete\") REFERENCES \"paquete\"(\"id\")); ";
        string query;
        string fkStr = "";
        bool fk = false;
        int count = 0;
        query = "BEGIN TRANSACTION;  CREATE TABLE IF NOT EXISTS \"" + tabla.nombre+"2\" (";
        foreach (var col in tabla.colum) {
            count++;
            query += "\""+col.nombre+"\"";
            if (col.nombre == "id") {
                query += " INTEGER";
            } else {
                if (col.size != "") {
                    query += " " + col.tipo + "(" + col.size + ")";
                } else {
                    query += " " + col.tipo + "";
                }
            }

            if (col.NN == "1") {
                query += " NOT NULL";
            }

            if (col.PK == "1") {
                query += " PRIMARY KEY";
            }

            if (col.AI == "1") {
                query += " AUTOINCREMENT";
            }

            if (col.fkTable != "") {
                fk = true;
                fkStr += " FOREIGN KEY(\""+col.nombre+"\") REFERENCES \""+col.fkTable+"\"(\"id\"),"; 
            }

            query += ",";
        }

        if (fk != true) {
            query = query.Substring(0, query.Length - 1);
        }
        else{
            fkStr = fkStr.Substring(0, fkStr.Length - 1);
            query += fkStr;
        }

        query += ");  COMMIT;";


        Debug.Log(query);
        ejecutarQuery(query, "Se creo la tabla correctamente");
    }

    void crearColumna(Data dataColum, string nombreTabla) {
        string query;
        query = "BEGIN TRANSACTION; "; 
        if (dataColum.size !="") {
            query += "ALTER TABLE " + nombreTabla + " ADD " + dataColum.nombre + " " + dataColum.tipo+";";
        } else {
            query += "ALTER TABLE " + nombreTabla + " ADD " + dataColum.nombre + " " + dataColum.tipo+"("+dataColum.size+");";
        }
        query += " COMMIT;";
        ejecutarQuery(query, "Se creo la columna correctamente");

    }

    void cambiarTipoColumna(Data dataColum, string nombreTabla) {
        string query;
        query = "ALTER TABLE " + nombreTabla + " ALTER COLUMN " + dataColum.nombre + " " + dataColum.tipo + ";";
        ejecutarQuery(query, "Se creo la columna correctamente");

    }

    public static string consultarTablas(string nombreTabla) {
        string query = "PRAGMA table_info("+nombreTabla+");";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    public static void ejecutarQuery(string queryEjecutar, string descripcion) {
        string query = queryEjecutar;
        var result = conexionDB.selectGeneral(query);
        Debug.Log(result);
        Debug.Log(descripcion);
    }
}
