using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class webServiceEstadistica : MonoBehaviour
{
    [Serializable]
    public class estadisticasDataSqLite {
        public string id = "";
        public string puntaje = "";
        public string numPreguntas = "";
        public string dificultad = "";
        public string idPaquete = "";
        public string idUsuario = "";
    }

    [Serializable]
    public class dataEstadistica {
        public estadisticasDataSqLite[] estadistica;
    }

    public static int insertarEstadisticasSqLite(string puntaje, int dificultad, int numPreguntas, string idPaquete, string idUsuario) {
        try {
            string query = "INSERT INTO estadistica (puntaje, numPreguntas, dificultad, idPaquete, idUsuario) VALUES (" + puntaje + ", "+ numPreguntas +", "+ dificultad +", "+ idPaquete +", " + idUsuario + ");";
            var result = conexionDB.alterGeneral(query);
            if (result == 1) {
                return 1;
            } else {
                return 0;
            }
        } catch (Exception ex) {
            Debug.LogError("Error en insertarIntentoSqLite: " + ex);
            return 0;
        }
    }

    public static int updatePuntajePaqueteSqlite(string puntaje, int numPreguntas, int dificultad, string idPaquete, string idUsuario) {
        string query = "UPDATE estadistica SET puntaje = " + puntaje + ", numPreguntas = "+ numPreguntas +" WHERE dificultad = "+ dificultad +" AND idPaquete = " + idPaquete + " AND idUsuario = " + idUsuario + ";";
        Debug.Log(query);
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int existPuntajePaqueteSqlite(int dificultad, string idPaquete, string idUsuario) {
        string query = "SELECT * FROM estadistica WHERE idPaquete = " + idPaquete + " AND  idUsuario = " + idUsuario +" AND dificultad = "+ dificultad;
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }

    public static int valPuntajePaqueteSqlite(string puntaje, int dificultad, string idPaquete, string idUsuario) {
        string query = "SELECT * FROM estadistica WHERE dificultad = "+ dificultad +" AND puntaje >" + puntaje + " AND idPaquete = " + idPaquete + " AND  idUsuario = " + idUsuario;
        Debug.Log(query);
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }

    public static string getPuntaje(int dificultad, string idPaquete, string idUsuario) {
        string query = "SELECT * FROM estadistica WHERE dificultad = "+ dificultad +" AND idPaquete = " + idPaquete + " AND  idUsuario = " + idUsuario;
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            estadisticasDataSqLite data = JsonUtility.FromJson<estadisticasDataSqLite>(result);
            return data.puntaje;
        } else {
            return "0";
        }
    }
}
