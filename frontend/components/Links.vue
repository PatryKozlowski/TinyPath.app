<template>
  <div :class="[countLinks! > 1 ? linksGridClass : linksFlexClass]">
    <Suspense>
      <slot />
    </Suspense>
  </div>
</template>

<script setup lang="ts">
const linksStore = useLinksStore()

const countLinks = computed(() => linksStore.links?.urls.length) ?? 0

const linksGridClass = 'grid lg:grid-cols-2 grid-cols-1 gap-4'
const linksFlexClass = 'flex'
</script>
