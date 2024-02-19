import { defineStore } from 'pinia'

interface GuestData {
  links: number
}

export const useGuestStore = defineStore({
  id: 'guest-store',
  state: () => ({
    isLoading: false,
    isSendEmailForLink: false,
    guestData: {
      links: 0
    } as GuestData | null
  }),
  getters: {
    links: (state) => state.guestData?.links
  },
  actions: {
    async loadGuestData() {
      this.isLoading = true
      const runTimeConfig = useRuntimeConfig()
      const MAX_GUEST_LINKS = runTimeConfig.public
        .MAX_GUEST_LINKS as unknown as number
      const GUEST_LINKS_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/Guest/GetGuestCommand`

      this.$patch((state) => {
        state.guestData = {
          links: MAX_GUEST_LINKS
        }
      })
      await $fetch<GuestData>(GUEST_LINKS_ENDPOINT_API, {
        method: 'GET',
        credentials: 'include'
      })
        .then((response) => {
          const restLinks = this.$state.guestData!.links - response.links
          this.$patch((state) => {
            state.guestData = {
              links: restLinks
            }
          })
        })
        .finally(() => {
          this.isLoading = false
        })
    }
  }
})
