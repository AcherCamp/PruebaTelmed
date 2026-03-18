<script setup>
defineProps({
  title: String,
  value: Number,
  color: String
})
</script>

<template>
  <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-all">
    <h2 class="text-3xl font-bold mb-2" :class="color">
      {{ value }}
    </h2>
    <p class="text-gray-500 text-sm uppercase tracking-wide">
      {{ title }}
    </p>
  </div>
</template>