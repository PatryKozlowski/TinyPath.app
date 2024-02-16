<template>
  <div
    class="w-full h-screen flex flex-col justify-center items-center bg-gray-700"
  >
    <div class="flex flex-col justify-center items-center">
      <Logo />
      <div class="flex items-center space-x-4 text-white">
        <div class="flex flex-col items-center mb-4">
          <p>Please check your email and confirm your email address</p>
          <p>Your link is active for 15 minutes</p>
        </div>
      </div>
      <Button class="space-x-2">
        <Icon name="lucide:mail" class="w-6 h-6" />
        <NuxtLink to="/auth/resend-email">Resend confirmation email</NuxtLink>
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
useHead({
  title: 'TinyPath - Confirmation',
  meta: [
    {
      name: 'description',
      content: 'Confirmation page'
    }
  ]
})
</script>
