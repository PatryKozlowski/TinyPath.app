import { defineStore } from 'pinia'

export const useLinkStore = defineStore({
  id: 'link-store',
  state: () => ({
    isLoading: true,
    link: {} as LinkResponse
  }),
  actions: {
    async loadLink(linkId: string) {
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      this.isLoading = true
      const { data } = await useAsyncData(() =>
        useRepository.loadLinkData(linkId)
      ).finally(() => {
        this.isLoading = false
      })

      if (data.value) {
        this.link = data.value
      } else {
        navigateTo('/dashboard')
      }
    }
  }
})
