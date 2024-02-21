<template>
  <div
    class="p-4 rounded-md shadow-lg bg-[#111827] min-h-28 hover:scale-95 transition-all duration-300 cursor-pointer"
    :class="{ 'shadow-yellow-500': isCustom }"
  >
    <div class="flex flex-col space-y-4">
      <span class="flex items-center space-x-2">
        <h2
          class="text-xl font-semiblod text-violet-500"
          :class="{ 'text-yellow-500': isCustom }"
          v-if="title"
        >
          {{ title }}
        </h2>
        <h2 class="text-xl font-semiblod text-pink-500" v-else>
          [Edit your link to create your title]
        </h2>
      </span>
      <span class="flex items-center space-x-2 text-white">
        <h2>Link:</h2>
        <p>{{ link }}</p>
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
const props = defineProps({
  title: {
    type: String
  },
  link: {
    type: String,
    required: true
  },
  isCustom: {
    type: Boolean
  }
})

const { link, title, isCustom } = props
</script>
