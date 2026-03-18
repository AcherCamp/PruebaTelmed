<template>

<div class="overflow-x-auto">

  <nav class="bg-[#4682B4] flex justify-center gap-4 py-4 px-4 text-white min-w-max">

    <RouterLink to="/" class="px-4 py-2 rounded hover:bg-white/20 whitespace-nowrap">
      Dashboard
    </RouterLink>

    <RouterLink to="/agenda" class="px-4 py-2 rounded hover:bg-white/20 whitespace-nowrap">
      Agenda
    </RouterLink>

    <RouterLink to="/consultas" class="px-4 py-2 rounded hover:bg-white/20 whitespace-nowrap">
      Consultas
    </RouterLink>

    <RouterLink to="/pacientes" class="px-4 py-2 rounded hover:bg-white/20 whitespace-nowrap">
      Pacientes
    </RouterLink>

    <RouterLink to="/historial" class="px-4 py-2 rounded hover:bg-white/20 whitespace-nowrap">
      Historial
    </RouterLink>

  </nav>
</div>
</template>