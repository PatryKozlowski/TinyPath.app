<template>
  <div class="space-y-4 py-4 flex flex-col h-full bg-[#111827] text-white">
    <div class="px-3 py-2 flex-1">
      <Logo :is-close="isClose" />
      <NuxtLink to="/dashboard/create-link">
        <Button class="w-full" size="sm" variant="secondary">
          <template v-if="isClose">
            <Icon name="lucide:plus" class="h-5 w-5" />
          </template>
          <template v-else> Create new </template>
        </Button>
      </NuxtLink>
      <Separator class="my-4" />
      <div class="space-y-3">
        <template v-for="(link, index) in sidebarLinks" :key="index">
          <NuxtLink
            :to="link.route"
            class="text-sm group flex justify-start w-full px-3 py-2 rounded-md hover:bg-gray-700 transition-colors duration-200"
            :class="{ 'bg-gray-600': currentRoute.path === link.route }"
          >
            <div
              class="flex items-center"
              :class="{ 'flex justify-center w-full': isClose }"
            >
              <Icon
                :name="link.icon"
                :class="{
                  'h-5 w-5': true,
                  [link.color]: true,
                  'mr-3': !isClose
                }"
              />
              <p
                :class="{ hidden: isClose }"
                class="font-medium whitespace-nowrap"
              >
                {{ link.label }}
              </p>
            </div>
          </NuxtLink>
          <Separator v-if="index === sidebarLinks.length - 2" class="my-4" />
        </template>
      </div>
    </div>
    <!-- Free content  -->
    <div
      class="px-4"
      v-if="!authStore?.user?.isAdmin && !authStore?.user?.isSubscribed"
    >
      <Button class="bg-violet-500 hover:bg-violet-400 w-full">
        <div
          class="flex items-center"
          :class="{ 'flex justify-center w-full': isClose }"
        >
          <Icon
            name="lucide:gem"
            class="w-4 h-4"
            :class="{ 'mr-2': !isClose }"
          />
          <span :class="{ hidden: isClose }"> Get premium </span>
        </div>
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
const currentRoute = useRoute()
const props = defineProps({
  closeSidebar: Boolean
})

const authStore = useAuthStore()

const isClose = ref(props.closeSidebar)

watch(
  () => props.closeSidebar,
  (newValue) => {
    isClose.value = newValue
  }
)
</script>
