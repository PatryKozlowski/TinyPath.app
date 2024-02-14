<template>
  <div class="flex items-center p-6 shadow-lg bg-[#111827] text-white">
    <!-- MobileSlider -->
    <div class="flex w-full justify-end">
      <DropdownMenu>
        <DropdownMenuTrigger as-child>
          <Button variant="secondary">
            <Icon name="lucide:user" class="w-5 h-5 mr-2" />
            Account
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent class="w-72 mr-2">
          <DropdownMenuLabel>My Account</DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuItem>patrykozlowski0@gmail.com</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem>Billing</DropdownMenuItem>
          <DropdownMenuItem>Team</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem class="cursor-pointer">
            <Icon name="lucide:log-out" class="w-5 h-5 mr-2" />
            Logout
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
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
</script>
