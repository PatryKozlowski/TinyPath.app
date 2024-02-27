<template>
  <div class="flex flex-col">
    <p class="text-2xl mb-4">{{ title }}</p>
    <div
      v-if="isPremium"
      class="flex items-center space-x-2"
      v-for="(count, item) in linkStoreCounts"
      :key="item"
    >
      <div class="flex w-2/12 space-x-2">
        <p>{{ item }}</p>
        <p>({{ count }})</p>
      </div>
      <dvi class="w-full p-2">
        <Progress :model-value="count" :max="linkStoreMaxCounts" />
      </dvi>
    </div>
    <Skeleton class="w-full h-16 rounded-xl" v-else />
  </div>
</template>

<script setup lang="ts">
import { Skeleton } from '@/components/ui/skeleton'

defineProps({
  isPremium: Boolean,
  linkStoreCounts: Object,
  linkStoreMaxCounts: Number,
  title: String
})
</script>
