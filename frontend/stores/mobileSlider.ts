export const useMobileSliderStore = defineStore({
  id: 'mobileSlider-store',
  state: () => ({
    isOpen: false
  }),

  actions: {
    toggleMobileSlider() {
      this.isOpen = !this.isOpen
    }
  }
})
