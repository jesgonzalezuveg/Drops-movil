using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;

public class webServiceDetalleIntento : MonoBehaviour
{
    /** Estructura que almacena los datos de las acciones desde SII
     */
    [Serializable]
    public class Data {
        public string id = "";
        public string correcto = "";
        public string idPregunta = "";
        public string idRespuesta = "";
        public string idIntento = "";
    }

    /**
     * Estructura que almacena los datos de las acciones desde SqLite
     */
    [Serializable]
    public class detalleIntentoDataSqLite {
        public string id = "";
        public string correcto = "";
        public string syncroStatus = "";
        public string idPregunta = "";
        public string idRespuesta = "";
        public string idIntento = "";
    }

    [Serializable]
    public class dataDetalleIntento {
        public detalleIntentoDataSqLite[] detalleIntento;
    }

    public static detalleIntentoDataSqLite[] getDetalleIntentosByIntento(string idIntento) {
        string query = "SELECT * FROM detalleIntento WHERE idIntento = " + idIntento + " AND syncroStatus = 0;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"detalleIntento\":[" + result + "]}";
            dataDetalleIntento data = JsonUtility.FromJson<dataDetalleIntento>(result);
            return data.detalleIntento;
        } else {
            return null;
        }
    }

    public static int updateSyncroStatusSqlite(string id, int sincroStatus) {
        string query = "UPDATE detalleIntento SET syncroStatus = '" + sincroStatus + "' WHERE id = '" + id + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que inseta los datos de la accion en la base de datos local
     * @param descripcion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     */
    public static int insertarDetalleIntentoSqLite(string correctoP, string idPreguntaP, string idRespuestaP, string idIntentoP) {
        try {
            if (correctoP == "true" || correctoP == "True") {
                correctoP = "1";
            } else {
                correctoP = "0";
            }
            string query = "INSERT INTO detalleIntento (correcto, syncroStatus, idPregunta, idRespuesta, idIntento) VALUES (" + correctoP + ", 0, " + idPreguntaP + ", " + idRespuestaP + ", " + idIntentoP + ");";
            var result = conexionDB.alterGeneral(query);
            if (result == 1) {
                return 1;
            } else {
                return 0;
            }
        } catch (Exception ex) {
            Debug.LogError("Error en insertarDetalleintento: " + ex);
            return 0;
        }
    }

    /** Función que consulta y trae los datos de la accion solicitada
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarDetalleIntentoByIdSqLite(string idP) {
        string query = "SELECT * FROM detalleIntento WHERE id = " + idP + ";";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    /** Función que consulta y trae los datos de la accion solicitada
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarDetalleIntentoByIdIntentoSqLite(string idP) {
        string query = "SELECT * FROM detalleIntento WHERE idIntento = " + idP + ";";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    /** Función para actualizar los datos de una accion
     * @param accion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     * @param idServer id de la accion en el servidor
     */
    public static int updateDetalleIntentoSqlite(string idP, string correctoP, string idPreguntaP, string idRespuestaP) {
        string query = "UPDATE detalleIntento SET correcto = " + correctoP + ", idPregunta =  "+ idPreguntaP + ", idRespuesta =  " + idRespuestaP + " WHERE id = " + idP + "";
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
    public static int updateSyncroStarusIntentoSqlite(string idP, int statusP) {
        string query = "UPDATE detalleIntento SET syncroStatus = " + statusP + " WHERE id = " + idP + "";
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
    public static int existDetalleIntentoSqlite(string idP) {
        string query = "SELECT * FROM detalleIntento WHERE id = " + idP + "";
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }

    public static int deleteDetalleIntentoSqlite(string id) {
        string query = "DELETE FROM detalleIntento WHERE id = " + id + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }
}
