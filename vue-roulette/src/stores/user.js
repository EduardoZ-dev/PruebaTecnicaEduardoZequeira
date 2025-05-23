import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    name: null,
    balance: 0,
    sessionBalance: 0,
    sessionId: null
  }),
  actions: {
    setUser({ name, balance, sessionId }) {
      this.name = name
      this.balance = balance
      this.sessionBalance = balance 
      this.sessionId = sessionId
    },
    updateBalance(newBalance) {
      this.balance = newBalance
    },
    updateSessionBalance(newBalance) {
      this.sessionBalance = newBalance
    },
    clearUser() {
      this.name = null
      this.balance = 0
      this.sessionBalance = 0
      this.sessionId = null
    }
  },
  getters: {
    isLoggedIn: (state) => !!state.name && !!state.sessionId
  }
}) 