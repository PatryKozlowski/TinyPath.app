<template>
  <div class="flex">
    <span class="flex items-center pl-3 mb-6">
      <div class="relative h-8 w-8 mr-4" :class="{ 'mr-0': isClose }">
        <img src="/image/tinypath-logo.svg" alt="Tinypath" />
      </div>
    </span>
    <h1 :class="{ hidden: isClose }" class="text-2xl font-bold">TinyPath</h1>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  isClose: Boolean,
});

const isClose = ref(props.isClose);

watch(
  () => props.isClose,
  (newValue) => {
    isClose.value = newValue;
  }
);
</script>
