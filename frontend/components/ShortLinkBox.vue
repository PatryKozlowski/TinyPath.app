<template>
  <div
    class="bg-green-300/80 border-2 border-green-500 p-4 rounded-md text-xs md:text-base max-h-20"
  >
    <LinkDisplay title="Created link" :url="createdLink" />
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  createdLink: String
})

const { createdLink } = props
</script>
