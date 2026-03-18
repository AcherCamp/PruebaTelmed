import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5050/api",
  headers: {
    "Content-Type": "application/json"
  }
});

// Enviar JWT automáticamente
api.interceptors.request.use(config => {
  const token = localStorage.getItem("token")

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

// Manejo global de errores
api.interceptors.response.use(
  response => response,
  error => {
    console.error("API ERROR:", error.response?.data || error.message)
    return Promise.reject(error)
  }
)

// CITAS
export const getCitas = async (params) => {
  const res = await api.get('/citas', { params })
  return res.data
}

export const getCalendario = async () => {
  const res = await api.get('/citas/calendario')
  return res.data
}

export const getCitaById = async (id) => {
  const res = await api.get(`/citas/${id}`)
  return res.data
}

export const getResumen = async () => {
  const res = await api.get('/citas/resumen')
  return res.data
}

export const updateCita = async (id, data) => {
  const res = await api.put(`/citas/${id}`, data)
  return res.data
}

export const confirmarCita = async (id) => {
  const res = await api.put(`/citas/${id}/confirmar`)
  return res.data
}

export const cancelarCita = async (id) => {
  const res = await api.put(`/citas/${id}/cancelar`)
  return res.data
}

//Métodos estado de consulta durante llamada
export const iniciarConsulta = async (id) => {
  const res = await api.put(`/citas/${id}/iniciar`)
  return res.data
}

export const finalizarConsulta = async (id) => {
  const res = await api.put(`/citas/${id}/finalizar`)
  return res.data
}

export default api