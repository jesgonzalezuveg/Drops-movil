using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;

public class webServiceIntento : MonoBehaviour {
    /** Estructura que almacena los datos de las acciones desde SII
     */
    [Serializable]
    public class Data {
        public string id = "";
        public string puntaje = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
        public string idLog = "";
    }

    /**
     * Estructura que almacena los datos de las acciones desde SqLite
     */
    [Serializable]
    public class intentoDataSqLite {
        public string id = "";
        public string puntaje = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
        public string syncroStatus = "";
        public string idLog = "";
    }

    [Serializable]
    public class dataIntento {
        public intentoDataSqLite[] intentos;
    }

    public static int updateSyncroStatusSqlite(string id, int sincroStatus) {
        string query = "UPDATE intento SET syncroStatus = '" + sincroStatus + "' WHERE id = '" + id + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int deleteIntentoSqlite(string id) {
        string query = "DELETE FROM intento WHERE id = " + id + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static intentoDataSqLite[] getIntentosByLog(string idLog) {
        string query = "SELECT * FROM intento WHERE idLog = " + idLog + " AND syncroStatus = 0;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"intentos\":[" + result + "]}";
            dataIntento data = JsonUtility.FromJson<dataIntento>(result);
            return data.intentos;
        } else {
            return null;
        }
    }

    /** Función que inseta los datos de la accion en la base de datos local
     * @param descripcion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     */
    public static int insertarIntentoSqLite(string puntaje, string usuario) {
        try {
            string id = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
            string idLog = webServiceLog.getLastLogSqLite(id);
            string query = "INSERT INTO intento (puntaje, fechaRegistro, fechaModificacion, syncroStatus, idLog) VALUES (" + puntaje + ", dateTime('now','localtime'), dateTime('now','localtime'), 0, " + idLog + ");";
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

    /** Función que consulta y trae los datos de la accion solicitada
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarIntentoByIdSqLite(string id) {
        string query = "SELECT * FROM intento WHERE id = " + id + ";";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    /** Función que consulta y trae los datos de la accion solicitada
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarIntentoByIdLogSqLite(string id) {
        string query = "SELECT * FROM intento WHERE idLog = " + id + ";";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    /** Función que consulta el id de la accion
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarUltimoIdIntentoByIdLogSqLite(string id) {
        string query = "SELECT id FROM intento WHERE idLog = " + id + "  ORDER by fechaRegistro DESC LIMIT 1;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            intentoDataSqLite data = JsonUtility.FromJson<intentoDataSqLite>(result);
            return data.id;
        } else {
            return "0";
        }
    }

    /** Función para actualizar los datos de una accion
     * @param accion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     * @param idServer id de la accion en el servidor
     */
    public static int updateIntentoSqlite(string id, string puntaje) {
        string query = "UPDATE intento SET puntaje = " + puntaje + ", fechaModificacion =  dateTime('now','localtime') WHERE id = " + id + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función para actualizar los datos de una accion
     * @param accion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     * @param idServer id de la accion en el servidor
     */
    public static int updateSyncroStarusIntentoSqlite(string id, int status) {
        string query = "UPDATE intento SET syncroStatus = " + status + ", fechaModificacion =  dateTime('now','localtime') WHERE id = " + id + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que verifica si la accion existe
     * @param usuario matricula o correo electronico del usuario
     */
    public static int existIntentoSqlite(string id) {
        string query = "SELECT * FROM intento WHERE id = " + id + "";
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }
}
