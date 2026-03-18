<script setup>
import { RouterView } from 'vue-router'
import Header from '@/components/layout/Header.vue'
import Navbar from '@/components/layout/Navbar.vue'
import Footer from '@/components/layout/FooterPage.vue'
import fondo from '@/assets/backgroundMD.jpg'
import headerBg from '@/assets/bannerHeader.jpg'
</script>

<template>
  <!-- Fondo general -->
  <div class="min-h-screen bg-slate-100 flex flex-col">

    <!-- HEADER -->
    <header
  class="relative text-white shadow-lg bg-cover bg-center"
  :style="{ backgroundImage: `url(${headerBg})` }">
  <!-- capa oscura -->
  <div class="absolute inset-0 bg-black/40"></div>
  <!-- contenido -->
  <div class="relative z-10">
    <Header />
  </div>
</header>

    <!-- NAVBAR -->
    <div class="bg-[#483D8B] sticky top-0 z-50">
      <Navbar />
    </div>

    <!--Main (contenido principal) -->
   <main class="relative flex-1 w-full px-4 md:px-8 py-6 overflow-hidden">
        <!-- Imagen de fondo -->
        <div
          class="absolute inset-0 bg-cover bg-center blur-sm scale-105"
          :style="{ backgroundImage: `url(${fondo})` }"
        ></div>

        <!-- Capa oscura suave -->
        <div class="absolute inset-0 bg-white/60"></div>

        <!-- Contenido -->
        <div class="relative z-10 w-full max-w-[100vw] overflow-x-hidden">
          <RouterView />
        </div>
  </main>

    <!-- FOOTER -->
    <Footer />
  </div>
</template>

<style>
/* Active link del navbar */
.router-link-active {
  background-color: var(--color-dark-slate-blue);
  color: var(--color-deep-sky-blue) !important;
  font-weight: bold;
  border-radius: 0.5rem;
}
</style>