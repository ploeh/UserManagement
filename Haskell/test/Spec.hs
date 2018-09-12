module Main where

import qualified Data.Map.Strict as Map
import Data.Map.Strict (Map)
import Control.Arrow
import Control.Monad.Trans.State
import Data.Maybe
import Text.Read (readMaybe)
import Test.Framework
import Test.Framework.Providers.QuickCheck2
import Test.QuickCheck
import UserManagement

main = defaultMain tests

tests = [
    testGroup "Properties" [
      testProperty "Users successfully connect" $ \
        user otherUser -> runStateTest $ do

        put $ Map.fromList [toDBEntry user, toDBEntry otherUser]

        actual <- post lookupUser updateUser (show $ userId user) (show $ userId otherUser)

        db <- get
        return $
          isOK actual &&
          any (elem otherUser . connectedUsers) (Map.lookup (userId user) db)
      
      ,
      testProperty "Users don't connect when user doesn't exist" $ \
        (Positive i) otherUser -> runStateTest $ do

        let db = Map.fromList [toDBEntry otherUser]
        put db
        let uniqueUserId = show $ userId otherUser + i

        actual <- post lookupUser updateUser uniqueUserId (show $ userId otherUser)

        assertPostFailure db actual
      
      ,
      testProperty "Users don't connect when other user doesn't exist" $ \
        (Positive i) user -> runStateTest $ do
        
        let db = Map.fromList [toDBEntry user]
        put db
        let uniqueOtherUserId = show $ userId user + i

        actual <- post lookupUser updateUser (show $ userId user) uniqueOtherUserId

        assertPostFailure db actual

      ,
      testProperty "Users don't connect when user Id is invalid" $ \
        s otherUser -> isIdInvalid s ==> runStateTest $ do

        let db = Map.fromList [toDBEntry otherUser]
        put db

        actual <- post lookupUser updateUser s (show $ userId otherUser)

        assertPostFailure db actual
      
      ,
      testProperty "Users don't connect when other user Id is invalid" $ \
        s user -> isIdInvalid s ==> runStateTest $ do

        let db = Map.fromList [toDBEntry user]
        put db

        actual <- post lookupUser updateUser (show $ userId user) s

        assertPostFailure db actual
    ]
  ]

type DB = Map Integer User

instance Arbitrary User where
  arbitrary = do
    id <- arbitrary
    return $ User id []

toDBEntry :: User -> (Integer, User)
toDBEntry = userId &&& id

lookupUser :: String -> State DB (Either UserLookupError User)
lookupUser s = do
  let maybeInt = readMaybe s :: Maybe Integer
  let eitherInt = maybe (Left InvalidId) Right maybeInt
  db <- get
  return $ eitherInt >>= maybe (Left NotFound) Right . flip Map.lookup db

updateUser :: User -> State DB ()
updateUser user = modify $ Map.insert (userId user) user

runStateTest :: State (Map k a) b -> b
runStateTest = flip evalState Map.empty

assertPostFailure :: (Eq s, Monad m) => s -> HttpResponse a -> StateT s m Bool
assertPostFailure stateBefore resp = do
  stateAfter <- get
  let stateDidNotChange = stateBefore == stateAfter
  return $ stateDidNotChange && isBadRequest resp

isIdInvalid :: String -> Bool
isIdInvalid s =
  let userInt = readMaybe s :: Maybe Integer
  in isNothing userInt