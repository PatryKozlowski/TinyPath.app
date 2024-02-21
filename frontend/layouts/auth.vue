<template>
  <div class="h-screen overflow-hidden">
    <Navbar />
    <div
      class="w-full h-full flex flex-col justify-center items-center bg-gray-700"
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
