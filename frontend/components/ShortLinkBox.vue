<template>
  <div
    class="bg-green-300/80 border-2 border-green-500 p-4 rounded-md text-xs md:text-base"
  >
    <div class="flex items-center md:space-x-4">
      <p class="hidden md:block">Your short link:</p>
      <div class="flex space-x-2">
        <p class="md:text-lg font-semibold">
          {{ createdLink }}
        </p>
        <a
          :href="createdLink"
          target="_blank"
          rel="noopener noreferrer"
          class="text-green-500 cursor-pointer hover:text-slate-500 transition-all duration-300"
        >
          <Icon class="md:w-5 md:h-5 w-4 h-4" name="lucide:external-link" />
        </a>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  createdLink: String
})

const { createdLink } = props
</script>
