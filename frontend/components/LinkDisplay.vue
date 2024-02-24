<template>
  <div class="flex space-x-2 items-center bg-slate-200 p-2 rounded-xl">
    <h2 class="truncate max-w-xs">{{ title }}:</h2>
    <p class="truncate max-w-xs">{{ url }}</p>
    <a
      :href="url"
      target="_blank"
      rel="noopener noreferrer"
      class="text-green-500 cursor-pointer hover:text-slate-500 transition duration-300 relative bottom-2"
    >
      <Icon class="md:w-5 md:h-5 w-4 h-4" name="lucide:external-link" />
    </a>
  </div>
</template>

<script setup lang="ts">
defineProps({
  title: String,
  url: String
})
</script>
