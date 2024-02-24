<template>
  <DashboardWrapper class="h-full">
    <template v-if="!links?.length && !linksStore.isLoading">
      <div
        class="w-full flex flex-col justify-center items-center min-h-[50vh] space-y-4"
      >
        <Icon name="lucide:frown" class="w-12 h-12 text-red-500" />
        <p>Sorry no links</p>
      </div>
    </template>
    <template v-else-if="links?.length && !linksStore.isLoading">
      <DashboardTitle title="Links" />
      <Links>
        <LinkItem
          v-for="link in links"
          :key="link.id"
          :id="link.id"
          :title="link.title!"
          :link="link.url"
          :isCustom="link.isCustom"
          :active="link.active"
        />
      </Links>
      <div class="flex justify-center">
        <Pagination v-if="linksStore.links?.totalPages! > 1" class="mt-6">
          <PaginationPrev
            @click="() => handlePageChange('prev')"
            :disabled="page === 1"
          />
          <PaginationNext
            @click="() => handlePageChange('next')"
            :disabled="page === linksStore.links?.totalPages"
          />
        </Pagination>
      </div>
    </template>
    <Spinner v-else class="flex w-full h-[50vh] justify-center items-center" />
  </DashboardWrapper>
</template>

<script setup lang="ts">
import {
  Pagination,
  PaginationNext,
  PaginationPrev
} from '@/components/ui/pagination'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const linksStore = useLinksStore()
const links = computed(() => linksStore.links?.urls)
const page = ref(1)

const handlePageChange = async (direction: 'next' | 'prev') => {
  const newValue = direction === 'next' ? page.value + 1 : page.value - 1

  page.value = Math.max(
    1,
    Math.min(newValue, linksStore.links?.totalPages || Infinity)
  )

  await linksStore.loadLinks(page.value)
}

onMounted(async () => {
  await linksStore.loadLinks(1).finally(() => {
    linksStore.isLoading = false
  })
})
</script>
