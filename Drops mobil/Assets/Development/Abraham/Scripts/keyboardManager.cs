using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyboardManager : MonoBehaviour {

    public GameObject teclasLetras;                ///< Conjunto botones que simulan las teclas de un teclado
    public GameObject teclasOtros;                 ///< Conjunto botones que simulan las teclas especiales de un teclado
    public GameObject teclasOtros2;                 ///< Conjunto botones que simulan las teclas especiales de un teclado
    string usuario = "";
    string nombre = "";
    string password = "";                   ///< string que contenera la verdadera contraseña, ya que el texto que aparecera en pantalla solo son asteriscos
    string password2 = "";

    bool isMinusculas = false;              ///< Bandera detecta si esta o no en mayusculas el teclado

    public GameObject[] inputs;

    GameObject inputActivo;
    public bool isPasswordInputActive;
    public bool isSecondPassword;
    public bool isNombre;
    public bool isUsuario;

    /** Función que se llama al inicio de la escena 
     * Inicia las referencias a lo GO
     */
    void Start() {
        inputActivo = inputs[0];
        teclasOtros.SetActive(false);
        teclasOtros2.SetActive(false);
    }
    private void Update() {
        if (isUsuario) {
            if (usuario.Length <= 0) {
                inputActivo.GetComponentInChildren<InputField>().text = "Correo o usuario";
            }
        }
        if (isNombre) {
            if (nombre.Length <= 0) {
                inputActivo.GetComponentInChildren<InputField>().text = "Nombre";
            }
        }
        if (isPasswordInputActive) {
            if (password.Length <= 0) {
                inputActivo.GetComponentInChildren<InputField>().text = "Ingresa tu contraseña";
            }
        }
        if (isSecondPassword) {
            if (password2.Length <= 0) {
                inputActivo.GetComponentInChildren<InputField>().text = "Confirma tu contraseña";
            }
        }
    }

    public void setUsuario(string usuario) {
        this.usuario = usuario;
    }

    public void setNombre(string nombre) {
        this.nombre = nombre;
    }

    public void setPassword(string pass) {
        password = pass;
    }

    public void setPassword2(string pass2) {
        password2 = pass2;
    }

    public void setIsUsuario(bool isUsuario) {
        clearAll();
        this.isUsuario = isUsuario;
    }

    public void setIsName(bool isNombre) {
        clearAll();
        this.isNombre = isNombre;
    }

    public void setIsPasswordInputActive(bool isPassword) {
        clearAll();
        isPasswordInputActive = isPassword;
    }

    public void setIsSecondPassword(bool isSecondPassword) {
        clearAll();
        this.isSecondPassword = isSecondPassword;
    }

    public void clearAll() {
        isNombre = false;
        isUsuario = false;
        isPasswordInputActive = false;
        isSecondPassword = false;
    }

    /** Función que se manda llamar al hacer click en una tecla del teclado
     * @param key, caracter que escribira en el input que se tenga seleccionado
     */
    public void GetKeyboardInput(string key) {
        if (isMinusculas) {
            key = key.ToLower();
        }
        if (isUsuario) {
            usuario += key;
            inputActivo.GetComponentInChildren<InputField>().text = "";
            inputActivo.GetComponentInChildren<InputField>().text = usuario;
        }
        if (isNombre) {
            nombre += key;
            inputActivo.GetComponentInChildren<InputField>().text = "";
            inputActivo.GetComponentInChildren<InputField>().text = nombre;
        }
        if (isPasswordInputActive) {
            password += key;
            inputActivo.GetComponentInChildren<InputField>().text = "";
            for (int i = 0; i < password.Length; i++) {
                inputActivo.GetComponentInChildren<InputField>().text += "*";
            }
        }
        if (isSecondPassword) {
            password2 += key;
            inputActivo.GetComponentInChildren<InputField>().text = "";
            for (int i = 0; i < password2.Length; i++) {
                inputActivo.GetComponentInChildren<InputField>().text += "*";
            }
        }
    }

    /** Función que elimina el ultimo caracter de el input que se tiene seleccionado
     * @param
     */
    public void DeleteChar() {
        if (inputActivo.GetComponentInChildren<InputField>().text != "") {
            inputActivo.GetComponentInChildren<InputField>().text = inputActivo.GetComponentInChildren<InputField>().text.Remove(inputActivo.GetComponentInChildren<InputField>().text.Length - 1);
            if (isUsuario) {
                if (usuario.Length > 0) {
                    usuario = usuario.Remove(usuario.Length - 1);
                }
                if (usuario.Length <= 0) {
                    inputActivo.GetComponentInChildren<InputField>().text = "Correo o usuario";
                }
            }
            if (isNombre) {
                if (nombre.Length > 0) {
                    nombre = nombre.Remove(nombre.Length - 1);
                }
                if (nombre.Length <= 0) {
                    inputActivo.GetComponentInChildren<InputField>().text = "Nombre";
                }
            }
            if (isPasswordInputActive) {
                if (password.Length > 0) {
                    password = password.Remove(password.Length - 1);
                }
                if (password.Length <= 0) {
                    inputActivo.GetComponentInChildren<InputField>().text = "Ingresa tu contraseña";
                }
            }
            if (isSecondPassword) {
                if (password2.Length > 0) {
                    password2 = password2.Remove(password2.Length - 1);
                }
                if (password2.Length <= 0) {
                    inputActivo.GetComponentInChildren<InputField>().text = "Confirma tu contraseña";
                }
            }
        }
    }

    /** Función que activa o desactiva las mayusculas
     * @param
     */
    public void BtnMinusculas() {
        isMinusculas = !isMinusculas;

        if (isMinusculas) {
            foreach (var t in teclasLetras.GetComponentsInChildren<InputField>()) {
                t.text = t.text.ToLower();
            }
        } else {
            foreach (var t in teclasLetras.GetComponentsInChildren<InputField>()) {
                t.text = t.text.ToUpper();
            }
        }
    }

    /** Función que activa o desactiva los caracteres especiales
     * @param
     */
    public void BtnLetras() {
        teclasOtros2.SetActive(false);
        teclasOtros.SetActive(false);
        teclasLetras.SetActive(true);
    }

    /** Función que activa o desactiva los caracteres especiales
     * @param
     */
    public void BtnOtros() {
        teclasOtros2.SetActive(false);
        teclasLetras.SetActive(false);
        teclasOtros.SetActive(true);
    }

    /** Función que activa o desactiva los caracteres especiales numero 2
     * @param
     */
    public void BtnOtros2() {
        teclasOtros.SetActive(false);
        teclasLetras.SetActive(false);
        teclasOtros2.SetActive(true);
    }


    /** Función que activa el input de matricula o correo usuario
     * @param
     */
    public void clickInput(GameObject input) {
        inputActivo.GetComponent<Image>().color = Color.white;
        inputActivo = input;
        inputActivo.GetComponent<Image>().color = new Color(1, 1, 0.8f);
    }


    /** Función que se activa cuando el usuario da click en el boton continuar
     * @param
     */
    public void login() {
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Cargando....");
        Debug.Log("Buscar en BD Local");

        // Consultar en BD local (sqlite)
        Debug.Log(inputs[0].GetComponentInChildren<InputField>().text + " - " + inputs[1].GetComponentInChildren<InputField>().text);
        password = inputs[1].GetComponentInChildren<InputField>().text;
        var usuario = webServiceUsuario.consultarLoginUsuarioSqLite(inputs[0].GetComponentInChildren<InputField>().text, password);
        if (usuario != null) {
            if (usuario.password != "") {
                Debug.Log("usuario no es null");
                GameObject.FindObjectOfType<appManager>().setNombre(usuario.nombre);
                GameObject.FindObjectOfType<appManager>().setUsuario(usuario.usuario);
                GameObject.FindObjectOfType<appManager>().setGradoEstudios(usuario.programa);
                GameObject.FindObjectOfType<appManager>().setImagen(usuario.imagen);
                int res = webServiceUsuario.updateSesionStatusSqlite(usuario.usuario, 1);
                if (res == 0) {
                    Debug.Log("Error en keyboardManager en linea 249");   
                    Debug.Log("Error al modificar el status de sesion del usuario");   
                }
                GameObject.FindObjectOfType<appManager>().isFirstLogin = true;
                StartCoroutine(GameObject.FindObjectOfType<appManager>().cambiarEscena("menuCategorias", "mainMenu"));
            } else {
                GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
                if (GameObject.Find("Mensaje")) {
                    GameObject.Find("Mensaje").GetComponent<Text>().text = "Contraseña incorrecta";
                }
            }
        } else {
            // Consultar en SII ambas BD
            StartCoroutine(webServiceUsuario.getUserData(inputs[0].GetComponentInChildren<InputField>().text, password));
        }
    }

    public void registrar() {

        usuario = this.inputs[0].GetComponentInChildren<InputField>().text;
        nombre = this.inputs[1].GetComponentInChildren<InputField>().text;
        password = this.inputs[2].GetComponentInChildren<InputField>().text;
        password2 = this.inputs[3].GetComponentInChildren<InputField>().text;

        if (usuario == "" || nombre == "" || password == "") {
            Debug.Log("Faltan campos por llenar");
            return;
        }
        if (usuario.Length < 8 || usuario.Length > 35) {
            Debug.Log("El usuario debe tener entre 8 y 35 caracteres");
            if (GameObject.Find("Mensaje")) {
                GameObject.Find("Mensaje").GetComponent<Text>().text = "El usuario debe tener entre 8 y 35 caracteres";
            }
            return;
        }
        string[] charAEliminar = { " ", "!", "\"", "#", "$", "%", "&", "\'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "[", "\\", "]", "^", "_", "`", "{", "|", "}", "ñ", "Ñ" };
        foreach (string caracter in charAEliminar) {
            string charPosition = caracter + "";
            if (password.Contains(charPosition)) {
                Debug.Log("La contraseña contiene caracteres invalidos");
                if (GameObject.Find("Mensaje")) {
                    GameObject.Find("Mensaje").GetComponent<Text>().text = "La contraseña contiene caracteres invalidos: " + caracter;
                }
                return;
            }
        }
        if (password.Length < 8 || password.Length > 50) {
            Debug.Log("La contraseña debe tener entre 8 y 50 caracteres");
            if (GameObject.Find("Mensaje")) {
                GameObject.Find("Mensaje").GetComponent<Text>().text = "La contraseña debe tener entre 8 y 50 caracteres";
            }
            return;
        }
        if (password != password2) {
            Debug.Log("Las contraseñas no coinciden");
            if (GameObject.Find("Mensaje")) {
                GameObject.Find("Mensaje").GetComponent<Text>().text = "Las contraseñas no coinciden";
            }
            return;
        }

        foreach (var input in inputs) {
            input.GetComponentInChildren<InputField>().text = "";
        }
        usuario = usuario.ToLower();
        StartCoroutine(webServiceUsuario.insertUsuario(usuario, nombre, password));
    }
}
