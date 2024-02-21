import type { $Fetch, NitroFetchRequest } from 'nitropack'
import type { CheckoutResponse } from '~/types'

const LOGIN_ENDPOINT_API = '/api/User/LoginCommand'
const REGISTER_ENDPOINT_API = '/api/User/CreateUserCommand'
const LOADED_USER_DATA_ENDPOINT_API = '/api/User/GetAuthenticatedUserCommand'
const LOGOUT_USER_ENDPOINT_API = '/api/User/Logout'
const CREATE_LINK_ENDPOINT_API = '/api/Link/CreateShortLinkCommand'
const CREATE_CUSTOM_LINK_ENDPOINT_API = '/api/Link/CreateCustomShortLinkCommand'
const CREATE_LINK_GUEST_ENDPOINT_API = '/api/Link/CreateShortLinkCommand'
const GUEST_LINKS_ENDPOINT_API = '/api/Guest/GetGuestCommand'
const GUEST_EMAIL_ENDPOINT_API =
  '/api/Link/SendLinkViewsEmailForGuestUserCommand'
const CHECKOUT_ENDPOINT_API = '/api/Stripe/CreateCheckoutSession'

export const repository = <T>(fetch: $Fetch<T, NitroFetchRequest>) => ({
  async login(formValue: LoginForm): Promise<LoginResponse> {
    return await fetch<LoginResponse>(LOGIN_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: formValue.email,
        password: formValue.password
      },
      onResponseError({ request, response, options }) {
        if (response._data.error === 'UserAlreadyLoggedIn') {
          navigateTo('/dashboard')
        }
      }
    })
  },

  async register(formValue: RegisterForm) {
    return await fetch<RegisterForm>(REGISTER_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: formValue.email,
        password: formValue.password,
        repeatPassword: formValue.repeatPassword
      }
    })
  },

  async loadedUserData(): Promise<LoadedUserDataResponse> {
    return await fetch<LoadedUserDataResponse>(LOADED_USER_DATA_ENDPOINT_API, {
      method: 'GET'
    })
  },

  async logout() {
    return await fetch<LogoutResponse>(LOGOUT_USER_ENDPOINT_API, {
      method: 'POST'
    })
  },

  async createShortLink(createLink: CreateLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_LINK_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title
      }
    })
  },

  async createShortLinkForGuest(createLink: CreateLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_LINK_GUEST_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title
      }
    })
  },

  async createCustomShortLink(createLink: CreateCustomLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_CUSTOM_LINK_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title,
        customLink: createLink.customCode
      }
    })
  },

  async loadGuestData() {
    return await fetch<GuestData>(GUEST_LINKS_ENDPOINT_API, {
      method: 'GET'
    })
  },

  async sendEmailForTrackingLink(guestEmail: GuestEmailForm) {
    return await fetch(GUEST_EMAIL_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: guestEmail.email
      }
    })
  },

  async createCheckoutSession(checkoutForm: CheckoutForm) {
    return await fetch<CheckoutResponse>(CHECKOUT_ENDPOINT_API, {
      method: 'POST',
      body: {
        priceCode: checkoutForm.priceCode
      }
    })
  }
})
