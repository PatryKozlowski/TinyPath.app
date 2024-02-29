<template>
  <div class="flex items-center p-6 shadow-lg bg-[#111827] text-white">
    <Button
      class="md:hidden"
      variant="secondary"
      @click="mobileSlider.toggleMobileSlider()"
      v-if="isAuthenticated"
    >
      <Icon name="lucide:menu" class="w-5 h-5" />
    </Button>
    <div class="flex w-full justify-end">
      <DropdownMenu v-if="isAuthenticated">
        <DropdownMenuTrigger as-child>
          <Button variant="secondary">
            <Icon name="lucide:user" class="w-5 h-5 mr-2" />
            Account
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent class="w-72 mr-2">
          <DropdownMenuLabel>My Account</DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuItem>
            {{ user?.email }}
            <Badge v-if="user?.isSubscribed" class="bg-yellow-500 ml-2">
              Premium
            </Badge>
            <Badge v-else-if="user?.isAdmin" class="bg-violet-500 ml-2">
              Admin
            </Badge>
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <Spinner v-if="isLoading" />
          <DropdownMenuItem
            class="cursor-pointer"
            v-if="user?.isSubscribed && !isLoading"
            @click="handleCreateBillingPortal"
            >Billing</DropdownMenuItem
          >
          <DropdownMenuSeparator />
          <DropdownMenuItem class="cursor-pointer" @click="authStore.logout()">
            <Icon name="lucide:log-out" class="w-5 h-5 mr-2" />
            Logout
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
      <NuxtLink v-else to="/auth/login">
        <Button variant="secondary"> Get started </Button>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu'

const authStore = useAuthStore()
const mobileSlider = useMobileSliderStore()
const { $api } = useNuxtApp()
const useRepository = repository($api)
const isLoading = ref(false)

const handleCreateBillingPortal = async () => {
  isLoading.value = true
  const data = await useRepository.createBillingPortal().finally(() => {
    isLoading.value = false
  })

  if (data.link) {
    window.location.href = data.link
  }
}
const { user, isAuthenticated } = storeToRefs(authStore)
</script>
