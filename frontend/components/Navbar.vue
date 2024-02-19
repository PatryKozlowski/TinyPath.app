<template>
  <div class="flex items-center p-6 shadow-lg bg-[#111827] text-white">
    <!-- MobileSlider -->
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
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem>Billing</DropdownMenuItem>
          <DropdownMenuItem>Team</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem class="cursor-pointer" @click="authStore.logout()">
            <!-- <Button variant="link"> -->
            <Icon name="lucide:log-out" class="w-5 h-5 mr-2" />
            Logout
            <!-- </Button> -->
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

const { user, isAuthenticated } = storeToRefs(authStore)
</script>
