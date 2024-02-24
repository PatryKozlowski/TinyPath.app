<template>
  <div class="h-full">
    <Navbar />
    <div
      class="w-full h-[calc(100vh-88px)] flex flex-col justify-center items-center bg-gray-700"
    >
      <div
        class="md:w-[400px] w-[350px] p-8 bg-slate-600 shadow-xl rounded-xl flex justify-center flex-col items-center"
      >
        <Logo />
        <Suspense>
          <slot />
        </Suspense>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts"></script>
