import { defineStore } from 'pinia'

interface Counts {
  [key: string]: number
}
export const useLinkStore = defineStore({
  id: 'link-store',
  state: () => ({
    isLoading: true,
    link: {} as LinkResponse,
    statistics: {} as LinkStatsResponse,
    browserCounts: {} as Counts,
    maxBrowserCounts: 0,
    platformCounts: {} as Counts,
    maxPlatformCounts: 0,
    deviceCounts: {} as Counts,
    maxDeviceCounts: 0,
    countryCounts: {} as Counts,
    maxCountryCounts: 0
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
    },

    browserStats() {
      this.browserCounts = {}
      this.maxBrowserCounts = 0
      this.statistics.browser.forEach((country) => {
        if (this.browserCounts[country]) {
          this.browserCounts[country]++
        } else {
          this.browserCounts[country] = 1
        }
      })

      this.maxBrowserCounts = Math.max(...Object.values(this.browserCounts))
    },

    deviceStats() {
      this.deviceCounts = {}
      this.maxDeviceCounts = 0
      this.statistics.device.forEach((country) => {
        if (this.deviceCounts[country]) {
          this.deviceCounts[country]++
        } else {
          this.deviceCounts[country] = 1
        }
      })

      this.maxDeviceCounts = Math.max(...Object.values(this.deviceCounts))
    },

    platformStats() {
      this.platformCounts = {}
      this.maxPlatformCounts = 0
      this.statistics.platform.forEach((country) => {
        if (this.platformCounts[country]) {
          this.platformCounts[country]++
        } else {
          this.platformCounts[country] = 1
        }
      })

      this.maxPlatformCounts = Math.max(...Object.values(this.platformCounts))
    },

    countryStats() {
      this.countryCounts = {}
      this.maxCountryCounts = 0
      this.statistics.country.forEach((country) => {
        if (this.countryCounts[country]) {
          this.countryCounts[country]++
        } else {
          this.countryCounts[country] = 1
        }
      })

      this.maxCountryCounts = Math.max(...Object.values(this.countryCounts))
    },

    async loadLinkStats(linkId: string) {
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      const { data } = await useAsyncData(() =>
        useRepository.getLinkStatsById(linkId)
      )
      if (data.value) {
        this.statistics = data.value
        this.browserStats()
        this.deviceStats()
        this.platformStats()
        this.countryStats()
      }
    }
  }
})
