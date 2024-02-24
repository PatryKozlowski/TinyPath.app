<template>
  <div class="w-full">
    <Navbar />
    <div class="h-[calc(100vh-88px)]">
      <div
        class="flex justify-center items-center h-full w-full bg-[#111827] text-white"
      >
        <div class="flex flex-col justify-center items-center space-y-6">
          <h1 class="text-4xl font-bold">Welcome to TinyPath</h1>
          <p class="text-center">
            Create a short link and share it with your friends.
          </p>
          <NuxtLink
            to="/dashboard/create-guest-link"
            class="p-2 bg-violet-500 rounded-xl w-1/2 flex items-center justify-center hover:bg-violet-400 transition-colors duration-300 cursor-pointer"
          >
            <Icon name="lucide:link-2" class="w-5 h-5 mr-2" />
            TRY IT !
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>
