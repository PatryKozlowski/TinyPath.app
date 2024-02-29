<template>
  <Spinner
    v-if="linkStore.isLoading"
    class="flex w-full h-[50vh] justify-center items-center"
  />
  <template v-else>
    <DashboardTitle title="Link statistics" />
    <p class="text-xl">
      Visitors:
      {{ isAdmin || isPremium ? linkStore.statistics.visits : visitors }}
    </p>
    <Button
      class="bg-violet-500 hover:bg-violet-400 w-full"
      v-if="!isPremium && !isAdmin"
    >
      <div class="flex items-center">
        <Icon name="lucide:gem" class="w-4 h-4 mr-2" />
        <span> Get premium </span>
      </div>
    </Button>
    <div
      class="shadow-md p-6 rounded-xl max-w-screen-lg w-full"
      :class="[isPremium || isAdmin ? 'bg-white' : 'bg-slate-700']"
    >
      <div class="w-full p-2 rounded-xl flex flex-col space-y-6">
        <LinkStatisticsItem
          :isPremium="isPremium || isAdmin"
          :linkStoreCounts="linkStore.browserCounts"
          :linkStoreMaxCounts="linkStore.maxBrowserCounts"
          title="Browsers"
        />
        <Separator class="my-6" />
        <LinkStatisticsItem
          :isPremium="isPremium || isAdmin"
          :linkStoreCounts="linkStore.platformCounts"
          :linkStoreMaxCounts="linkStore.maxPlatformCounts"
          title="Platforms"
        />
        <Separator class="my-6" />
        <LinkStatisticsItem
          :isPremium="isPremium || isAdmin"
          :linkStoreCounts="linkStore.countryCounts"
          :linkStoreMaxCounts="linkStore.maxCountryCounts"
          title="Country"
        />
        <Separator class="my-6" />
        <LinkStatisticsItem
          :isPremium="isPremium || isAdmin"
          :linkStoreCounts="linkStore.deviceCounts"
          :linkStoreMaxCounts="linkStore.maxDeviceCounts"
          title="Devices"
        />
      </div>
    </div>
  </template>
</template>

<script setup lang="ts">
const linkStore = useLinkStore()
const authStore = useAuthStore()
const router = useRoute()
const linkId = ref('')
const { $api } = useNuxtApp()
const useRepository = repository($api)
const visitors = ref(0)
const isPremium = computed(() => authStore.user?.isSubscribed)
const isAdmin = computed(() => authStore.user?.isAdmin)

onMounted(async () => {
  linkId.value = router.params.id as string
  if (isAdmin || isPremium) {
    await linkStore.loadLinkStats(linkId.value)
  } else {
    const data = await useRepository.getLinkViewsById(linkId.value)

    if (data) {
      visitors.value = data.linkViewsCount
    }
  }
})
</script>
