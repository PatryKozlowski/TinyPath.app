<template>
  <div
    class="md:hidden flex flex-col z-[9999] bg-[#111827] text-white absolute top-0 left-0 w-full h-full"
    :class="{
      'translate-x-0': mobileSlider.isOpen,
      '-translate-x-full': !mobileSlider.isOpen,
      'transition-all': true,
      'duration-300': true
    }"
  >
    <div class="flex justify-end p-4">
      <Button variant="secondary" @click="mobileSlider.toggleMobileSlider()">
        <Icon name="lucide:x" class="w-5 h-5" />
      </Button>
    </div>
    <div
      class="w-full p-12 h-full flex flex-col space-y-6"
      @click="mobileSlider.toggleMobileSlider()"
    >
      <NuxtLink to="/dashboard/create-link">
        <Button
          class="w-full flex items-center justify-center"
          size="sm"
          variant="secondary"
        >
          <Icon name="lucide:plus" class="h-5 w-5 mr-2" />
          Create new
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
            <div class="flex items-center">
              <Icon
                :name="link.icon"
                :class="{
                  'h-5 w-5 mr-2': true,
                  [link.color]: true
                }"
              />
              <p class="font-medium whitespace-nowrap">
                {{ link.label }}
              </p>
            </div>
          </NuxtLink>
          <Separator v-if="index === sidebarLinks.length - 2" class="my-4" />
        </template>
        <div
          class="flex h-full items-end"
          v-if="!authStore?.user?.isAdmin && !authStore?.user?.isSubscribed"
        >
          <Button class="bg-violet-500 hover:bg-violet-400 w-full">
            <div class="flex items-center">
              <Icon name="lucide:gem" class="w-4 h-4 mr-2" />
              <span> Get premium </span>
            </div>
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const currentRoute = useRoute()
const authStore = useAuthStore()
const mobileSlider = useMobileSliderStore()
</script>
