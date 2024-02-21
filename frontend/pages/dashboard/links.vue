<template>
  <DashboardWrapper>
    <DashboardTitle title="Links" />
    <Links>
      <LinkItem
        title="My custom link"
        link="https://tinypath.kozlowskip.pl/4x@cxs$"
        isCustom
      />
      <LinkItem
        title="My custom link"
        link="https://tinypath.kozlowskip.pl/4x@cxs$"
      />
      <LinkItem link="https://tinypath.kozlowskip.pl/4x@cxs$" />
    </Links>
    <div class="flex justify-center">
      <Pagination
        v-slot="{ page }"
        :total="9"
        :sibling-count="3"
        :default-page="1"
      >
        <PaginationList v-slot="{ items }" class="flex items-center gap-1">
          <PaginationFirst />
          <PaginationPrev />

          <template v-for="(item, index) in items">
            <PaginationListItem
              v-if="item.type === 'page'"
              :key="index"
              :value="item.value"
              as-child
            >
              <Button
                class="w-10 h-10 p-0"
                :variant="item.value === page ? 'default' : 'outline'"
              >
                {{ item.value }}
              </Button>
            </PaginationListItem>
            <PaginationEllipsis v-else :key="item.type" :index="index" />
          </template>

          <PaginationNext />
          <PaginationLast />
        </PaginationList>
      </Pagination>
    </div>
  </DashboardWrapper>
</template>

<script setup lang="ts">
import {
  Pagination,
  PaginationEllipsis,
  PaginationFirst,
  PaginationLast,
  PaginationList,
  PaginationListItem,
  PaginationNext,
  PaginationPrev
} from '@/components/ui/pagination'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const page = ref(3)
</script>
