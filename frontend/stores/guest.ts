import { defineStore } from 'pinia'

export const useGuestStore = defineStore({
  id: 'guest-store',
  state: () => ({
    isSendEmailForLink: false,
    guestData: {
      links: 0,
      blocked: false,
      blockedUntil: null
    } as GuestData | null
  }),
  getters: {
    links: (state) => state.guestData?.links
  },
  actions: {
    async loadGuestData() {
      const runTimeConfig = useRuntimeConfig()
      const maxLinksForGuest = runTimeConfig.public
        .MAX_GUEST_LINKS as unknown as number
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      this.guestData = {
        links: maxLinksForGuest,
        blocked: false,
        blockedUntil: null
      }
      const data = await useRepository.loadGuestData()
      const restLinks = maxLinksForGuest - data.links
      if (data) {
        this.guestData = {
          links: restLinks,
          blocked: data.blocked,
          blockedUntil: data.blockedUntil
        }
      }
    }
  }
})
