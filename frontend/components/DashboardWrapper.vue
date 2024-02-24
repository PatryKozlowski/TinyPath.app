<template>
  <div class="flex w-full flex-col">
    <div class="w-full flex md:justify-center">
      <div class="md:w-3/4 md:max-w-screen-lg w-full space-y-4 h-full">
        <Suspense>
          <slot />
        </Suspense>
      </div>
    </div>
  </div>
</template>
