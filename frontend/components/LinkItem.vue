<template>
  <div
    :class="[
      'flex',
      'flex-col',
      'space-y-4',
      ' p-4',
      'rounded-md',
      'shadow-lg',
      ' bg-[#111827]',
      'min-h-28',
      isCustom ? 'shadow-yellow-500' : '',
      countLinks! > 1 ? 'min-w-md' : 'w-full',
    ]"
  >
    <div class="flex justify-between items-center">
      <Badge :class="badgeClass">
        <span class="flex items-center">
          <Icon :name="statusIconBadge" class="w-4 h-4 mr-2" />
          {{ active ? 'Active' : 'Inactive' }}
        </span>
      </Badge>
      <NuxtLink
        :to="`/dashboard/link/${id}`"
        class="inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium h-10 px-4 py-2 bg-violet-500 text-white hover:bg-violet-400 transition-colors duration-300"
      >
        <Icon name="lucide:settings" class="w-4 h-4 ml-auto" />
      </NuxtLink>
    </div>
    <h2
      :class="[
        'truncate max-w-xs',
        'text-xl font-semibold',
        title ? 'text-violet-500' : 'text-pink-500',
        isCustom && title ? 'text-yellow-500' : ''
      ]"
    >
      {{ title || '[Edit your link to create your title]' }}
    </h2>
    <LinkDisplay title="Link" :url="link" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'

const statusIconBadge = computed(() =>
  props.active ? 'lucide:shield-check' : 'lucide:shield-x'
)

const badgeClass = computed(() => [
  'mb-2',
  props.active ? 'bg-green-500' : 'bg-red-500'
])

const props = defineProps({
  id: {
    type: String,
    required: true
  },
  title: String,
  link: {
    type: String,
    required: true
  },
  isCustom: {
    type: Boolean,
    default: false
  },
  active: {
    type: Boolean,
    default: true
  }
})

const linksStore = useLinksStore()

const countLinks = computed(() => linksStore.links?.urls.length) ?? 0
</script>
