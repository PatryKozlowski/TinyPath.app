<template>
  <div class="h-full relative">
    <div :class="sidebarClass" v-if="isAuthenticated">
      <Sidebar :close-sidebar="sidebarClosed" />
      <Button
        class="hidden z-[90] md:flex absolute top-12 left-[270px] rounded-full transition-all duration-300"
        :class="{
          'left-[64px]': sidebarClosed
        }"
        variant="secondary"
        size="iconSlider"
        @click="toggleSidebar"
      >
        <Icon
          :name="sidebarClosed ? 'lucide:arrow-right' : 'lucide:arrow-left'"
          class="w-5 h-5"
        />
      </Button>
    </div>
    <div :class="wrapperClass">
      <Navbar />
      <main class="p-10">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
const sidebarClosed = ref(false)
const authStore = useAuthStore()
const { isAuthenticated } = storeToRefs(authStore)

const toggleSidebar = () => {
  sidebarClosed.value = !sidebarClosed.value
}

const sidebarClass = computed(() => ({
  relative: true,
  hidden: true,
  'h-full': true,
  'md:flex': true,
  'md:flex-col': true,
  'md:fixed': true,
  'md:inset-y-0': true,
  'z-[80]': true,
  'bg-gray-900': true,
  'transition-width': true,
  'duration-300': true,
  'md:w-72': !sidebarClosed.value,
  'md:w-20': sidebarClosed.value
}))

const wrapperClass = computed(() => ({
  'transition-width': true,
  'duration-300': true,
  'md:pl-72': !sidebarClosed.value && isAuthenticated.value,
  'md:pl-20': sidebarClosed.value
}))
</script>
