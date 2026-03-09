<template>
    <div> 
    <form :class="colorForm" class="p-6 rounded-xl shadow-md w-80"  id="form">
        <h3>LOGIN</h3><br>
        
        <input id="iptDa" class="border border-black" type="text" placeholder="Email" required><br>
        <br><input id="iptDa" class="border border-black" type="password" placeholder="Contraseña" required><br>
        <br><div class="recordar"><input class="input1" type="checkbox"><label>Recordarme</label> <br><label>Olvidaste tu Contraseña</label><br></div>
        <br><button >Iniciar Sesion</button><br>
    </form></div>
</template>
<script>
export default {
    name: 'login',
    data() {
        return{
            colorForm: 'bg-cyan-500'
        }
    },

    methods: {
        cambiarColor() {
            this.colorForm = 'bg-green-200'
        }
    }
}
</script>