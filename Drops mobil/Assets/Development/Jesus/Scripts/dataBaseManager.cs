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


    public TextAsset jsonFile;
    public string[] ordenTablas;
    string path;
    string jsonString;
    bool tableFound = false;
    bool columFound = false;

    private void Awake() {
        //La variable ordenTablas debe contener todas las tablas de la db, de lo contrario no las creará aunque esten en el json.
        //El orden de la tablas es importante. Si se crea una tabla con fk y no existe la tabla a la cual hace referencia la fk, esto causara un error
        ordenTablas = new string[] {"usuario", "catalogoAcciones", "catalogoCatgoriaPaquete", "catalogoEjercicio", "codigo", "paquete", "descarga", "aviso", "estadistica", "log", "preferencias", "intento", "registros", "pregunta", "respuesta", "detalleIntento" };
        tableFound = false;
        columFound = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ejecutarQuery("PRAGMA foreign_keys=off;", "Se desactivaron FKs");
        //path = Application.streamingAssetsPath + "/dataBase.json";
        //jsonString = File.ReadAllText(path);
        jsonString = jsonFile.text;
        Debug.Log(jsonFile.text);
        Tablas tablasJson = JsonUtility.FromJson<Tablas>(jsonString);
        for (int i = 0; i < ordenTablas.Length; i++) {
            foreach (var tabJson in tablasJson.tabla) {
                Debug.Log(tabJson.nombre);
                if (tabJson.nombre == ordenTablas[i]) {
                    string res = consultarTablas(tabJson.nombre);
                    if (res == "0") {
                        //Debug.Log("No encontro la tabla " + tabJson.nombre);
                        //Debug.Log("Crear tabla en la bd");
                        queryCrearTabla(tabJson);
                    } else {
                        string columnasMutuas = "";
                        bool recrearTabla = false;
                        tableFound = true;
                        ColumnasDB columnasDB = JsonUtility.FromJson<ColumnasDB>("{\"colums\" : [" + res + "]}");
                        //Debug.Log("NOMBRE  " + tabJson.nombre);
                        //Verificar que todas las columnas coincidan en la bd como en el json
                        foreach (var colJson in tabJson.colum) {
                            foreach (var colDB in columnasDB.colums) {
                                string tipoColDB = colDB.type.Split('(')[0];

                                if (tipoColDB == "int" || tipoColDB == "INTEGER") {
                                    tipoColDB = "int";
                                }
                                if (colDB.name == colJson.nombre && tipoColDB == colJson.tipo) {
                                    //Debug.Log("NOMBRE COLUMNA DB " + colDB.name);
                                    //Debug.Log("NOMBRE COLUMNA JSON " + colJson.nombre);
                                    //Debug.Log("TIPO COLUMNA DB " + tipoColDB);
                                    //Debug.Log("TIPO COLUMNA JSON " + colJson.tipo);
                                    //Debug.Log("-------------------------------------");

                                    columnasMutuas += colJson.nombre+", ";
                                    columFound = true;
                                } else {
                                    //Debug.Log("El nombre de la columna actual del json no coincide.");
                                }
                            }
                            if (columFound == false && recrearTabla == false) {
                                //Debug.Log("Agregar columna a la bd y recrear tabla");
                                recrearTabla = true;
                            }
                            if (columFound == true) {
                                //Debug.Log(columFound);
                            }
                            columFound = false;
                        }

                        //Debug.Log(recrearTabla);
                        if (recrearTabla == true) {
                            columnasMutuas = columnasMutuas.Substring(0, columnasMutuas.Length - 2);
                            remplazarTabla(tabJson, columnasMutuas);
                            //Continuar aqui
                        } else {
                            //Debug.Log("NO RECREAR TABLA "+ tabJson.nombre);
                        }
                    }
                } else {
                    //Debug.Log("El nombre de la tabla actual no coincide.");
                }
            }
        }

        ejecutarQuery("PRAGMA foreign_keys=on;", "Se activaron FKs");
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
        query = "BEGIN TRANSACTION;  CREATE TABLE IF NOT EXISTS \"" + tabla.nombre+"\" (";
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


        //Debug.Log(query);
        execQuery(query, "Se creo la tabla correctamente");
    }

    /*void crearColumna(Data dataColum, string nombreTabla) {
        string query;
        query = "BEGIN TRANSACTION; "; 
        if (dataColum.size !="") {
            query += "ALTER TABLE " + nombreTabla + " ADD " + dataColum.nombre + " " + dataColum.tipo+";";
        } else {
            query += "ALTER TABLE " + nombreTabla + " ADD " + dataColum.nombre + " " + dataColum.tipo+"("+dataColum.size+");";
        }
        query += " COMMIT;";
        ejecutarQuery(query, "Se creo la columna correctamente");

    }*/

    /*void cambiarTipoColumna(Data dataColum, string nombreTabla) {
        string query;
        query = "ALTER TABLE " + nombreTabla + " ALTER COLUMN " + dataColum.nombre + " " + dataColum.tipo + ";";
        ejecutarQuery(query, "Se creo la columna correctamente");

    }*/

    void remplazarTabla(Columnas tabla, string columnasMutuas) {
        string query;
        string fkStr = "";
        bool fk = false;
        int count = 0;
        query = "BEGIN TRANSACTION; " +
            "ALTER TABLE "+ tabla.nombre + " RENAME TO _"+ tabla.nombre + "_old; " +
            "CREATE TABLE \"" + tabla.nombre + "\" (";
        foreach (var col in tabla.colum) {
            count++;
            query += "\"" + col.nombre + "\"";
            if (col.nombre == "id") {
                query += " INTEGER";
            } else {
                if (col.size != "") {
                    query += " " + col.tipo + "(" + col.size + ")";
                } else {
                    query += " " + col.tipo + "";
                }
            }

            //if (col.NN == "1") {
            //    query += " NOT NULL";
            //}

            if (col.PK == "1") {
                query += " PRIMARY KEY";
            }

            if (col.AI == "1") {
                query += " AUTOINCREMENT";
            }

            if (col.fkTable != "") {
                fk = true;
                fkStr += " FOREIGN KEY(\"" + col.nombre + "\") REFERENCES \"" + col.fkTable + "\"(\"id\"),";
            }

            query += ",";
        }

        if (fk != true) {
            query = query.Substring(0, query.Length - 1);
        } else {
            fkStr = fkStr.Substring(0, fkStr.Length - 1);
            query += fkStr;
        }

        query += "); ";
        if (columnasMutuas != null && columnasMutuas != "") {
            query += "INSERT INTO " + tabla.nombre + "(" + columnasMutuas + ") " +
            "SELECT " + columnasMutuas + " FROM _" + tabla.nombre + "_old; ";
        }
            query +="DROP TABLE _"+ tabla.nombre + "_old; " +
            "COMMIT;";


        Debug.Log(query);
        execQuery(query, "Se altero la tabla correctamente");
    }

    public static string consultarTablas(string nombreTabla) {
        string query = "PRAGMA table_info("+nombreTabla+");";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    public static void ejecutarQuery(string queryEjecutar, string descripcion) {
        string query = queryEjecutar;
        var result = conexionDB.selectGeneral(query);
        //Debug.Log(result);
        Debug.Log(descripcion);
    }

    public static void execQuery(string queryEjecutar, string descripcion) {
        string query = queryEjecutar;
        var result = conexionDB.alterGeneral(query);
        //Debug.Log(result);
        Debug.Log(descripcion);
    }
}
