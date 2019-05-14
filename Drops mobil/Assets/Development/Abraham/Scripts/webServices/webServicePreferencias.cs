using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class webServicePreferencias : MonoBehaviour {

    /** 
     * Estructura que almacena los datos de una preferencia
     */
    [Serializable]
    public class preferenceData {
        public string id = "";
        public string idUsuario = "";
        public string numeroPreguntas = "";
        public string mascota = "";
        public string fondo = "";
    }

    [Serializable]
    public class Data {
        public preferenceData[] logs;
    }

    public static int updatePreferenciaSqlite(string usuario, float numeroPreguntas, bool mascota, int fondo) {
        int mascotaValue;
        if (mascota) {
            mascotaValue = 1;
        } else {
            mascotaValue = 0;
        }
        string idUsuario = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
        string query = "UPDATE preferencias SET numeroPreguntas = '" + numeroPreguntas + "', mascota = '" + mascotaValue + "', fondo = '" + fondo + "' WHERE idUsuario = '" + idUsuario + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que inserta los datos de las preferencias de usuario
     * @param idUsuario id de usuario logeado
     */
    public static int insertarPreferenciaSqLite(string usuario) {
        string id = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
        string query = "INSERT INTO preferencias (idUsuario, numeroPreguntas, mascota, fondo) VALUES ('" + id + "', 5, 1, 0);";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static preferenceData getPreferenciaSqLite(string usuario) {
        string id = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
        string query = "SELECT * FROM preferencias WHERE idUsuario = " + id + ";";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            preferenceData data = JsonUtility.FromJson<preferenceData>(result);
            return data;
        } else {
            return null;
        }
    }

}
