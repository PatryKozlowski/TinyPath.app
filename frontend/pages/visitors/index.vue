<template>
  <div
    class="w -full h-screen bg-slate-700 flex justify-center items-center p-4"
  >
    <div
      class="rounded-xl bg-[#111827] shadow-xl p-4 flex flex-col space-y-2 md:w-[400px] w-[350px] text-white"
      v-if="!isLoading"
    >
      <template v-if="url">
        <span class="md:flex md:items-center text-sm md:space-x-1">
          <p>Your link:</p>
          <a
            class="hover:text-slate-500 transition-colors duration-300 font-semibold"
            :href="url"
            target="_blank"
            rel="noopener noreferrer"
            >{{ url }}</a
          >
        </span>
        <p>Visitors: {{ visitors }}</p>
      </template>
      <div v-else>
        <p class="text-center">Link not found</p>
      </div>
    </div>
    <Spinner v-else class="text-center" />
  </div>
</template>

<script setup lang="ts">
interface LinkStats {
  linkUrl: string
  linkViewsCount: number
}

definePageMeta({
  middleware: ['check-linkid']
})

useHead({
  title: 'TinyPath - Link visitors',
  meta: [
    {
      name: 'description',
      content: 'Visitors page'
    }
  ]
})

const route = useRoute()
const linkId = ref('')
const visitors = ref(0)
const url = ref('')
const isLoading = ref(true)
const { $api } = useNuxtApp()
const useRepository = repository($api)

onMounted(async () => {
  linkId.value = route.query.linkId as string

  const data = await useRepository
    .getLinkViewsById(linkId.value)
    .finally(() => {
      isLoading.value = false
    })

  if (data) {
    url.value = data.linkUrl
    visitors.value = data.linkViewsCount
  }
})
</script>
