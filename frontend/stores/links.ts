import { defineStore } from 'pinia'
import type { LinksResponse } from '~/types'

export const useLinksStore = defineStore({
  id: 'links-store',
  state: () => ({
    isLoading: true,
    links: null as LinksResponse | null
  }),
  actions: {
    async loadLinks(pageNo: number = 1, pageSize: number = 4) {
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      const data = await useRepository.loadLinksData(pageNo, pageSize)

      if (data) {
        this.links = data
      }
    }
  }
})
